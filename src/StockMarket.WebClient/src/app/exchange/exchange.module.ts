import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";

import { AppMaterialModule } from "@root/material.module";

import { ExchangeRouterModule } from "./exchange.routes";
import { ExchangeComponent } from "./exchange.component";
import { ListComponent } from "./list/list.component";
import { ViewComponent } from "./view/view.component";
import { NewComponent } from "./new/new.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        AppMaterialModule,
        HttpClientModule,
        ExchangeRouterModule
    ],
    declarations: [ExchangeComponent, ListComponent, ViewComponent, NewComponent]
})
export class ExchangeModule {}
