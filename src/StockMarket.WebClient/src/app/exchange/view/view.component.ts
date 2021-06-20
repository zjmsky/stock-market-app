import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";

import { tap } from "rxjs/operators";

import { ExchangeService } from "@root/_services/exchange.service";
import { Exchange } from "@root/_models/exchange";

@Component({
    templateUrl: "./view.component.html",
    styleUrls: ["./view.component.css"]
})
export class ViewComponent implements OnInit {
    private _exchangeService: ExchangeService;
    private _router: Router;
    private _route: ActivatedRoute;

    public exchange?: Exchange;

    constructor(
        exchangeService: ExchangeService,
        route: ActivatedRoute,
        router: Router
    ) {
        this._exchangeService = exchangeService;
        this._route = route;
        this._router = router;
    }

    ngOnInit() {
        const routeParams = this._route.snapshot.paramMap;
        const exchangeCode = routeParams.get("exchangeCode")!;

        this._exchangeService
            .getOne(exchangeCode)
            .pipe(tap((exchange) => (this.exchange = exchange)))
            .subscribe();
    }

    deleteExchange() {
        const exchangeCode = this._route.snapshot.paramMap.get("exchangeCode")!;
        this._exchangeService
            .delete(exchangeCode)
            .pipe(tap((result) => this._router.navigateByUrl("/exchanges")))
            .subscribe();
    }
}
