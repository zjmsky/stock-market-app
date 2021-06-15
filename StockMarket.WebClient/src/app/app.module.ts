import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HttpClientModule } from "@angular/common/http";

import { AppMaterialModule } from "./material.module";
import { AppRouterModule } from "./app.routes";

import { AppComponent } from "./app.component";
import { ToolbarComponent } from "@root/shared/toolbar/toolbar.component";
import { SidenavComponent } from "@root/shared/sidenav/sidenav.component";

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        AppMaterialModule,
        AppRouterModule
    ],
    declarations: [AppComponent, ToolbarComponent, SidenavComponent],
    bootstrap: [AppComponent]
})
export class AppModule {}
