import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";

import { tap } from "rxjs/operators";

import { Company } from "@root/_models/company";
import { Listing } from "@root/_models/listing";
import { CompanyService } from "@root/_services/company.service";
import { ListingService } from "@root/_services/listing.service";

@Component({
    templateUrl: "./view.component.html",
    styleUrls: ["./view.component.css"]
})
export class ViewComponent implements OnInit {
    private _companyService: CompanyService;
    private _listingService: ListingService;
    private _router: Router;
    private _route: ActivatedRoute;

    public company?: Company;
    public listings?: Listing[];

    constructor(
        companyService: CompanyService,
        listingService: ListingService,
        route: ActivatedRoute,
        router: Router
    ) {
        this._companyService = companyService;
        this._listingService = listingService;
        this._route = route;
        this._router = router;
    }

    ngOnInit() {
        const routeParams = this._route.snapshot.paramMap;
        const companyCode = routeParams.get("companyCode")!;

        this._companyService
            .getOne(companyCode)
            .pipe(tap((company) => (this.company = company)))
            .subscribe();

        this._listingService
            .getByCompany(companyCode)
            .pipe(tap((listings) => (this.listings = listings)))
            .subscribe();
    }

    deleteCompany() {
        this.listings?.forEach((listing) => {
            this._listingService
                .delete(listing.exchangeCode, listing.tickerSymbol)
                .subscribe();
        });

        this._companyService
            .delete(this.company!.companyCode)
            .pipe(tap((result) => this._router.navigateByUrl("/companies")))
            .subscribe();
    }
}
