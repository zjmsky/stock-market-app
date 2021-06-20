import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { tap } from "rxjs/operators";

import { SectorService } from "@root/_services/sector.service";
import { Sector } from "@root/_models/sector";

@Component({
    templateUrl: "./edit.component.html",
    styleUrls: ["./edit.component.css"]
})
export class EditComponent implements OnInit {
    private _formBuilder: FormBuilder;
    private _router: Router;
    private _sectorService: SectorService;

    public sectorCode: string | null;
    public editForm?: FormGroup;

    constructor(
        formBuilder: FormBuilder,
        router: Router,
        route: ActivatedRoute,
        sectorService: SectorService
    ) {
        this._formBuilder = formBuilder;
        this._router = router;
        this._sectorService = sectorService;

        this.sectorCode = route.snapshot.paramMap.get("sectorCode");
    }

    ngOnInit(): void {
        this.editForm = this._formBuilder.group({
            sectorCode: ["", [Validators.required]],
            name: ["", [Validators.required]],
            description: [""]
        });

        if (this.sectorCode != null) {
            this._sectorService
                .getOne(this.sectorCode)
                .pipe(
                    tap((data) => {
                        this.editForm?.setValue({
                            sectorCode: data.sectorCode,
                            name: data.name,
                            description: data.description
                        });
                        this.editForm!.controls["sectorCode"].disable();
                    })
                )
                .subscribe();
        }
    }

    onSubmit(): void {
        if (this.editForm?.invalid) return;

        const form = this.editForm!;

        const sector = new Sector();
        sector.sectorCode = form.get("sectorCode")!.value;
        sector.name = form.get("name")!.value;
        sector.description = form.get("description")!.value;

        if (this.sectorCode == null) {
            this._sectorService
                .create(sector)
                .pipe(tap((response) => this._router.navigateByUrl("/sectors")))
                .subscribe();
        } else {
            this._sectorService
                .update(sector)
                .pipe(tap((response) => this._router.navigateByUrl("/sectors")))
                .subscribe();
        }
    }
}
