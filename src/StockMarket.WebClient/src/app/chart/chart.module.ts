import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { ChartsModule } from "ng2-charts";

import { AppMaterialModule } from "@root/material.module";

import { ChartRouterModule } from "./chart.routes";
import { ChartComponent } from "./chart.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        ChartsModule,
        AppMaterialModule,
        HttpClientModule,
        ChartRouterModule
    ],
    declarations: [ChartComponent]
})
export class ChartModule {}
