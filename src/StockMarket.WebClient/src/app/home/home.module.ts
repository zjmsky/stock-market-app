import { NgModule } from "@angular/core";

import { HomeRouterModule } from "./home.routes";
import { HomeComponent } from "./home.component";

@NgModule({
    imports: [HomeRouterModule],
    declarations: [HomeComponent]
})
export class HomeModule {}
