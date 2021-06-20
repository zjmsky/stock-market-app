import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

import { tap } from 'rxjs/operators';

import { ExchangeService } from "@root/_services/exchange.service";
import { Exchange } from "@root/_models/exchange";

@Component({
    templateUrl: "./view.component.html",
    styleUrls: ["./view.component.css"]
})
export class ViewComponent implements OnInit {
    private _exchangeService: ExchangeService;
    private _route: ActivatedRoute;

    public exchange?: Exchange;

    constructor(exchangeService: ExchangeService, route: ActivatedRoute) {
        this._exchangeService = exchangeService;
        this._route = route;
    }

    ngOnInit() {
        const routeParams = this._route.snapshot.paramMap;
        const exchangeCode = routeParams.get('exchangeCode')!;

        this._exchangeService.getOne(exchangeCode)
            .pipe(tap(exchange => this.exchange = exchange))
            .subscribe();
    }
}