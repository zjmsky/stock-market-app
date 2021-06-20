import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { HttpResponse } from "@angular/common/http";

import { tap } from "rxjs/operators";

import { ExchangeService } from "@root/_services/exchange.service";
import { Exchange } from "@root/_models/exchange";

@Component({
    templateUrl: "./list.component.html",
    styleUrls: ["./list.component.css"]
})
export class ListComponent {
    private _exchangeService: ExchangeService;
    private _router: Router;

    public itemCount: number = 10;
    public pageSize: number = 10;

    public displayedColumns = ["exchangeCode", "name", "country"];
    public dataSource: Array<Exchange>;

    constructor(exchangeService: ExchangeService, router: Router) {
        this._exchangeService = exchangeService;
        this._router = router;
        this.dataSource = new Array();
        this.onPageChange({ pageIndex: 1 });
    }

    onPageChange(event: any) {
        this._exchangeService
            .getList(event.pageIndex, this.pageSize)
            .pipe(tap(response => this.handleResponse(response)))
            .subscribe();
    }

    onRowClick(row: Exchange) {
        const exchangeCode = row.exchangeCode.toLowerCase();
        const destUrl = `/exchanges/${exchangeCode}`;
        this._router.navigateByUrl(destUrl);
    }

    handleResponse(response: HttpResponse<Exchange[]>) {
        const countRepr = response.headers.get('Total-Count')!;
        this.itemCount = Number.parseInt(countRepr);

        this.dataSource = response.body!;
    }
}
