import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

const loadHomeModule = () =>
    import("@root/home/home.module").then((x) => x.HomeModule);
const loadAccountModule = () =>
    import("@root/account/account.module").then((x) => x.AccountModule);

const routes: Routes = [
    { path: "", loadChildren: loadHomeModule },
    { path: "account", loadChildren: loadAccountModule }
];

@NgModule({
    declarations: [],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRouterModule {}
