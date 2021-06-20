import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { ExchangeComponent } from "./exchange.component";
import { ListComponent } from "./list/list.component";
import { ViewComponent } from "./view/view.component";
import { EditComponent } from "./edit/edit.component";

const routes: Routes = [
    {
        path: "",
        component: ExchangeComponent,
        children: [
            { path: "", component: ListComponent },
            { path: "new", component: EditComponent },
            { path: ":exchangeCode", component: ViewComponent },
            { path: "edit/:exchangeCode", component: EditComponent }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ExchangeRouterModule {}
