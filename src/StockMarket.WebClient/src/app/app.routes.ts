import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { AuthGuard } from "./_middlewares/auth.guard";

const loadHomeModule = () =>
    import("@root/home/home.module").then((x) => x.HomeModule);
const loadAccountModule = () =>
    import("@root/account/account.module").then((x) => x.AccountModule);
const loadDashboardModule = () =>
    import("@root/dashboard/dashboard.module").then((x) => x.DashboardModule);
const loadExchangeModule = () =>
    import("@root/exchange/exchange.module").then((x) => x.ExchangeModule);
const loadSectorModule = () =>
    import("@root/sector/sector.module").then((x) => x.SectorModule);
const loadCompanyModule = () =>
    import("@root/company/company.module").then((x) => x.CompanyModule);
const loadChartModule = () =>
    import("@root/chart/chart.module").then((x) => x.ChartModule);

const routes: Routes = [
    { path: "", loadChildren: loadHomeModule },
    { path: "account", loadChildren: loadAccountModule },
    {
        path: "dashboard",
        loadChildren: loadDashboardModule,
        canActivate: [AuthGuard]
    },
    {
        path: "exchanges",
        loadChildren: loadExchangeModule,
        canActivate: [AuthGuard]
    },
    {
        path: "sectors",
        loadChildren: loadSectorModule,
        canActivate: [AuthGuard]
    },
    {
        path: "companies",
        loadChildren: loadCompanyModule,
        canActivate: [AuthGuard]
    },
    {
        path: "charts",
        loadChildren: loadChartModule,
        canActivate: [AuthGuard]
    }
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouterModule {}
