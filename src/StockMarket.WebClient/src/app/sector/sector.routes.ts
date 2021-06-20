import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { SectorComponent } from "./sector.component";
import { ListComponent } from "./list/list.component";
import { ViewComponent } from "./view/view.component";
import { EditComponent } from "./edit/edit.component";

const routes: Routes = [
    {
        path: "",
        component: SectorComponent,
        children: [
            { path: "", component: ListComponent },
            { path: "new", component: EditComponent },
            { path: ":sectorCode", component: ViewComponent },
            { path: "edit/:sectorCode", component: EditComponent }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SectorRouterModule {}
