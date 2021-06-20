import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";

import { AppMaterialModule } from "@root/material.module";

import { CompanyRouterModule } from "./company.routes";
import { CompanyComponent } from "./company.component";
import { ListComponent } from "./list/list.component";
import { ViewComponent } from "./view/view.component";
import { EditComponent } from "./edit/edit.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        AppMaterialModule,
        HttpClientModule,
        CompanyRouterModule
    ],
    declarations: [
        CompanyComponent,
        ListComponent,
        ViewComponent,
        EditComponent
    ]
})
export class CompanyModule {}
