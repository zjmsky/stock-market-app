import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { HttpResponse } from "@angular/common/http";

import { tap } from "rxjs/operators";

import { SectorService } from "@root/_services/sector.service";
import { Sector } from "@root/_models/sector";

@Component({
    templateUrl: "./list.component.html",
    styleUrls: ["./list.component.css"]
})
export class ListComponent {
    private _sectorService: SectorService;
    private _router: Router;

    public itemCount: number = 10;
    public pageSize: number = 10;

    public displayedColumns = ["sectorCode", "name"];
    public dataSource: Array<Sector>;

    constructor(sectorService: SectorService, router: Router) {
        this._sectorService = sectorService;
        this._router = router;
        this.dataSource = new Array();
        this.onPageChange({ pageIndex: 1 });
    }

    onPageChange(event: any) {
        this._sectorService
            .getList(event.pageIndex, this.pageSize)
            .pipe(tap((response) => this.handleResponse(response)))
            .subscribe();
    }

    onRowClick(row: Sector) {
        const sectorCode = row.sectorCode.toLowerCase();
        const destUrl = `/sectors/${sectorCode}`;
        this._router.navigateByUrl(destUrl);
    }

    handleResponse(response: HttpResponse<Sector[]>) {
        const countRepr = response.headers.get("Total-Count")!;
        this.itemCount = Number.parseInt(countRepr);

        this.dataSource = response.body!;
    }
}
