import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { tap } from "rxjs/operators";

import { ExchangeService } from "@root/_services/exchange.service";
import { Exchange } from "@root/_models/exchange";
import { Address } from "@root/_models/address";

@Component({
    templateUrl: "./edit.component.html",
    styleUrls: ["./edit.component.css"]
})
export class EditComponent implements OnInit {
    private _formBuilder: FormBuilder;
    private _router: Router;
    private _exchangeService: ExchangeService;

    public exchangeCode: string | null;
    public editForm?: FormGroup;

    constructor(
        formBuilder: FormBuilder,
        router: Router,
        route: ActivatedRoute,
        exchangeService: ExchangeService
    ) {
        this._formBuilder = formBuilder;
        this._router = router;
        this._exchangeService = exchangeService;

        this.exchangeCode = route.snapshot.paramMap.get("exchangeCode");
    }

    ngOnInit(): void {
        this.editForm = this._formBuilder.group({
            exchangeCode: ["", [Validators.required]],
            countryCode: ["", [Validators.required]],
            name: ["", [Validators.required]],
            description: [""],
            address_line1: [""],
            address_line2: [""],
            address_city: [""],
            address_state: [""],
            address_country: [""],
            address_phone: [""],
            address_email: ["", [Validators.email]]
        });

        if (this.exchangeCode != null) {
            this._exchangeService
                .getOne(this.exchangeCode)
                .pipe(
                    tap((data) => {
                        this.editForm?.setValue({
                            exchangeCode: data.exchangeCode,
                            countryCode: data.countryCode,
                            name: data.name,
                            description: data.description,
                            address_line1: data.address.line1,
                            address_line2: data.address.line2,
                            address_city: data.address.city,
                            address_state: data.address.state,
                            address_country: data.address.country,
                            address_phone: data.address.phoneNumber,
                            address_email: data.address.email
                        });
                        this.editForm!.controls["exchangeCode"].disable();
                    })
                )
                .pipe()
                .subscribe();
        }
    }

    onSubmit(): void {
        if (this.editForm?.invalid) return;

        const form = this.editForm!;

        const exchange = new Exchange();
        exchange.exchangeCode = form.get("exchangeCode")!.value;
        exchange.countryCode = form.get("countryCode")!.value;
        exchange.name = form.get("name")!.value;
        exchange.description = form.get("description")!.value;

        exchange.address = new Address();
        exchange.address.line1 = form.get("address_line1")!.value;
        exchange.address.line2 = form.get("address_line2")!.value;
        exchange.address.city = form.get("address_city")!.value;
        exchange.address.state = form.get("address_state")!.value;
        exchange.address.country = form.get("address_country")!.value;
        exchange.address.phoneNumber = form.get("address_phone")!.value;
        exchange.address.email = form.get("address_email")!.value;

        if (this.exchangeCode == null) {
            this._exchangeService
                .create(exchange)
                .pipe(
                    tap((response) => this._router.navigateByUrl("/exchanges"))
                )
                .subscribe();
        } else {
            this._exchangeService
                .update(exchange)
                .pipe(
                    tap((response) => this._router.navigateByUrl("/exchanges"))
                )
                .subscribe();
        }
    }
}
