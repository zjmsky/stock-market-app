import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";

import { tap } from "rxjs/operators";

import { SectorService } from "@root/_services/sector.service";
import { Sector } from "@root/_models/sector";

@Component({
    templateUrl: "./view.component.html",
    styleUrls: ["./view.component.css"]
})
export class ViewComponent implements OnInit {
    private _sectorService: SectorService;
    private _router: Router;
    private _route: ActivatedRoute;

    public sector?: Sector;

    constructor(
        sectorService: SectorService,
        route: ActivatedRoute,
        router: Router
    ) {
        this._sectorService = sectorService;
        this._route = route;
        this._router = router;
    }

    ngOnInit() {
        const routeParams = this._route.snapshot.paramMap;
        const sectorCode = routeParams.get("sectorCode")!;

        this._sectorService
            .getOne(sectorCode)
            .pipe(tap((sector) => (this.sector = sector)))
            .subscribe();
    }

    deleteSector() {
        const sectorCode = this._route.snapshot.paramMap.get("sectorCode")!;
        this._sectorService
            .delete(sectorCode)
            .pipe(tap((result) => this._router.navigateByUrl("/sectors")))
            .subscribe();
    }
}
