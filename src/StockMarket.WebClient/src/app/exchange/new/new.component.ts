import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";

import { ExchangeService } from "@root/_services/exchange.service";
import { Exchange } from "@root/_models/exchange";
import { Address } from "@root/_models/address";

@Component({
    templateUrl: "./new.component.html",
    styleUrls: ["./new.component.css"]
})
export class NewComponent implements OnInit {
    private _formBuilder: FormBuilder;
    private _router: Router;
    private _exchangeService: ExchangeService;

    public newForm?: FormGroup;

    // private destroy$ = new Subject<void>();

    constructor(formBuilder: FormBuilder, router: Router, exchangeService: ExchangeService) {
        this._formBuilder = formBuilder;
        this._router = router;
        this._exchangeService = exchangeService;
    }

    ngOnInit(): void {
        this.newForm = this._formBuilder.group({
            exchangeCode: ["", [Validators.required]],
            countryCode: ["", [Validators.required]],
            name: ["", [Validators.required]],
            description: [""],
            address_line1: [""],
            address_line2: [""],
            address_state: [""],
            address_country: [""],
            address_phone: [""],
            address_email: ["", [Validators.email]],
        })
    }

    onSubmit(): void {
        if (this.newForm?.invalid) return;
        const form = this.newForm!;
        const exchange = new Exchange();
        exchange.exchangeCode = form.get("exchangeCode")!.value;
        exchange.countryCode = form.get("countryCode")!.value;
        exchange.name = form.get("name")!.value;
        exchange.description = form.get("description")!.value;
        exchange.address = new Address();
        exchange.address.line1 = form.get("address_line1")!.value;
        exchange.address.line2 = form.get("address_line2")!.value;
        exchange.address.state = form.get("address_state")!.value;
        exchange.address.country = form.get("address_country")!.value;
        exchange.address.phoneNumber = form.get("address_phone")!.value;
        exchange.address.email = form.get("address_email")!.value;
        this._exchangeService.create(exchange)
            .subscribe(response => this._router.navigateByUrl('/exchanges'));
    }
}