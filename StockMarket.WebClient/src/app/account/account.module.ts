import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { AppMaterialModule } from "@root/material.module";

import { AccountRouterModule } from "./account.routes";
import { AccountComponent } from "./account.component";
import { LoginComponent } from "./login/login.component";
import { RegisterComponent } from "./register/register.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        AppMaterialModule,
        AccountRouterModule
    ],
    declarations: [AccountComponent, LoginComponent, RegisterComponent]
})
export class AccountModule {}
