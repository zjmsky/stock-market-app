import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { AppMaterialModule } from "./material.module";
import { AppRouterModule } from "./app.routes";

import { AppComponent } from "./app.component";
import { ToolbarComponent } from "@root/shared/toolbar/toolbar.component";

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        AppMaterialModule,
        AppRouterModule
    ],
    declarations: [AppComponent, ToolbarComponent],
    bootstrap: [AppComponent]
})
export class AppModule {}
