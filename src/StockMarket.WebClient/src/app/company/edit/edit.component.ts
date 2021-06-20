import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { tap } from "rxjs/operators";

import { Company } from "@root/_models/company";
import { Listing } from "@root/_models/listing";
import { CompanyService } from "@root/_services/company.service";
import { ListingService } from "@root/_services/listing.service";

@Component({
    templateUrl: "./edit.component.html",
    styleUrls: ["./edit.component.css"]
})
export class EditComponent implements OnInit {
    private _formBuilder: FormBuilder;
    private _router: Router;
    private _companyService: CompanyService;
    private _listingService: ListingService;

    private _listings: Listing[];

    public companyCode: string | null;
    public editForm?: FormGroup;
    public tickerForm?: FormGroup;

    constructor(
        formBuilder: FormBuilder,
        router: Router,
        route: ActivatedRoute,
        companyService: CompanyService,
        listingService: ListingService
    ) {
        this._formBuilder = formBuilder;
        this._router = router;
        this._companyService = companyService;
        this._listingService = listingService;

        this._listings = [];

        this.companyCode = route.snapshot.paramMap.get("companyCode");
    }

    ngOnInit(): void {
        this.editForm = this._formBuilder.group({
            companyCode: ["", [Validators.required]],
            sectorCode: ["", [Validators.required]],
            name: ["", [Validators.required]],
            description: [""],
            turnover: [""],
            ceo: [""]
        });

        this.tickerForm = this._formBuilder.group({
            ticker1: [""],
            ticker2: [""],
            ticker3: [""]
        });

        if (this.companyCode != null) {
            this._companyService
                .getOne(this.companyCode)
                .pipe(
                    tap((data) => {
                        this.editForm?.setValue({
                            companyCode: data.companyCode,
                            sectorCode: data.sectorCode,
                            name: data.name,
                            description: data.description,
                            turnover: data.turnover,
                            ceo: data.ceo
                        });
                        this.editForm!.controls["companyCode"].disable();
                    })
                )
                .subscribe();

            this._listingService
                .getByCompany(this.companyCode)
                .pipe(
                    tap((listings) => {
                        this._listings = listings;
                        const ticker1 = listings[0]
                            ? listings[0].exchangeCode +
                              ":" +
                              listings[0].tickerSymbol
                            : null;
                        const ticker2 = listings[1]
                            ? listings[1].exchangeCode +
                              ":" +
                              listings[1].tickerSymbol
                            : null;
                        const ticker3 = listings[2]
                            ? listings[2].exchangeCode +
                              ":" +
                              listings[2].tickerSymbol
                            : null;
                        this.tickerForm?.setValue({
                            ticker1,
                            ticker2,
                            ticker3
                        });
                    })
                )
                .subscribe();
        }
    }

    onSubmit(): void {
        if (this.editForm?.invalid) return;
        if (this.tickerForm?.invalid) return;

        const tickerForm = this.tickerForm!;
        const newListings: string[] = [];
        if (tickerForm.get("ticker1")!.value)
            newListings.push(tickerForm.get("ticker1")!.value);
        if (tickerForm.get("ticker2")!.value)
            newListings.push(tickerForm.get("ticker2")!.value);
        if (tickerForm.get("ticker3")!.value)
            newListings.push(tickerForm.get("ticker3")!.value);
        const oldListings = this._listings.map(
            (x) => x.exchangeCode + ":" + x.tickerSymbol
        );

        const shouldAdd: string[] = [];
        const shouldRemove: string[] = [];
        for (const listing of oldListings) {
            if (!newListings.includes(listing)) shouldRemove.push(listing);
        }
        for (const listing of newListings) {
            if (!oldListings.includes(listing)) shouldAdd.push(listing);
        }

        for (const listing of shouldAdd) {
            const listingObj = new Listing();
            const listingParts = listing.split(":");
            listingObj.exchangeCode = listingParts[0];
            listingObj.tickerSymbol = listingParts[1];
            listingObj.companyCode = this.companyCode!;
            this._listingService.create(listingObj).toPromise();
        }
        for (const listing of shouldRemove) {
            const listingParts = listing.split(":");
            const exchangeCode = listingParts[0];
            const tickerSymbol = listingParts[1];
            this._listingService.delete(exchangeCode, tickerSymbol).toPromise();
        }

        const form = this.editForm!;

        const company = new Company();
        company.companyCode = form.get("companyCode")!.value;
        company.sectorCode = form.get("sectorCode")!.value;
        company.name = form.get("name")!.value;
        company.description = form.get("description")!.value;
        company.turnover = form.get("turnover")!.value;
        company.ceo = form.get("ceo")!.value;

        if (this.companyCode == null) {
            this._companyService
                .create(company)
                .pipe(
                    tap((response) => this._router.navigateByUrl("/companies"))
                )
                .subscribe();
        } else {
            this._companyService
                .update(company)
                .pipe(
                    tap((response) => this._router.navigateByUrl("/companies"))
                )
                .subscribe();
        }
    }
}
