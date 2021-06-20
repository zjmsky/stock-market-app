import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { CompanyComponent } from "./company.component";
import { ListComponent } from "./list/list.component";
import { ViewComponent } from "./view/view.component";
import { EditComponent } from "./edit/edit.component";

const routes: Routes = [
    {
        path: "",
        component: CompanyComponent,
        children: [
            { path: "", component: ListComponent },
            { path: "new", component: EditComponent },
            { path: ":companyCode", component: ViewComponent },
            { path: "edit/:companyCode", component: EditComponent }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class CompanyRouterModule {}
