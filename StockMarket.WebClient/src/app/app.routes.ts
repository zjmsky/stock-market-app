import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

const loadHomeModule = () =>
    import("@root/home/home.module").then((x) => x.HomeModule);
const loadAccountModule = () =>
    import("@root/account/account.module").then((x) => x.AccountModule);
const loadDashboardModule = () =>
    import("@root/dashboard/dashboard.module").then((x) => x.DashboardModule);

const routes: Routes = [
    { path: "", loadChildren: loadHomeModule },
    { path: "account", loadChildren: loadAccountModule },
    { path: "dashboard", loadChildren: loadDashboardModule }
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouterModule {}
