import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { ExchangeComponent } from "./exchange.component";
import { ListComponent } from "./list/list.component";

const routes: Routes = [
    {
        path: "",
        component: ExchangeComponent,
        children: [
            { path: "list", component: ListComponent },
            // { path: "register", component: RegisterComponent }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ExchangeRouterModule {}
