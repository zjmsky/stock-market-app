import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { MaterialModule } from "./material.module";
import { RoutingModule } from "./routing.module";

import { AppComponent } from "./app.component";
import { ToolbarComponent } from "./toolbar/toolbar.component";
import { LoginComponent } from "./login/login.component";

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        RoutingModule,
        MaterialModule
    ],
    declarations: [AppComponent, ToolbarComponent, LoginComponent],
    bootstrap: [AppComponent]
})
export class AppModule {}
