import { NgModule } from "@angular/core";

import { AccountRouterModule } from "./account.routes";
import { AccountComponent } from "./account.component";
import { LoginComponent } from "./login/login.component";
import { SignupComponent } from "./signup/signup.component";

@NgModule({
    imports: [AccountRouterModule],
    declarations: [AccountComponent, LoginComponent, SignupComponent]
})
export class AccountModule {}
