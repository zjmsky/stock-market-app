import { NgModule } from "@angular/core";

import { DashboardRouterModule } from "./dashboard.routes";
import { DashboardComponent } from "./dashboard.component";

@NgModule({
    imports: [DashboardRouterModule],
    declarations: [DashboardComponent]
})
export class DashboardModule {}
