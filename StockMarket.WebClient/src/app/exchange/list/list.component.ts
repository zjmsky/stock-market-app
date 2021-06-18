import { Component } from "@angular/core";

import { ExchangeService } from "@root/_services/exchange.service";

@Component({
    templateUrl: "./list.component.html",
    styleUrls: ["./list.component.css"]
})
export class ListComponent {
    private _exchangeService: ExchangeService;

    constructor(exchangeService: ExchangeService) {
        this._exchangeService = exchangeService;
    }
}