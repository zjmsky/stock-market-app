import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { HttpResponse } from "@angular/common/http";

import { tap } from "rxjs/operators";

import { CompanyService } from "@root/_services/company.service";
import { Company } from "@root/_models/company";

@Component({
    templateUrl: "./list.component.html",
    styleUrls: ["./list.component.css"]
})
export class ListComponent {
    private _companyService: CompanyService;
    private _router: Router;

    public itemCount: number = 10;
    public pageSize: number = 10;

    public displayedColumns = ["companyCode", "name", "sectorCode"];
    public dataSource: Array<Company>;

    constructor(companyService: CompanyService, router: Router) {
        this._companyService = companyService;
        this._router = router;
        this.dataSource = new Array();
        this.onPageChange({ pageIndex: 1 });
    }

    onPageChange(event: any) {
        this._companyService
            .getList(event.pageIndex, this.pageSize)
            .pipe(tap((response) => this.handleResponse(response)))
            .subscribe();
    }

    onRowClick(row: Company) {
        const companyCode = row.companyCode.toLowerCase();
        const destUrl = `/companies/${companyCode}`;
        this._router.navigateByUrl(destUrl);
    }

    handleResponse(response: HttpResponse<Company[]>) {
        const countRepr = response.headers.get("Total-Count")!;
        this.itemCount = Number.parseInt(countRepr);

        this.dataSource = response.body!;
    }
}
