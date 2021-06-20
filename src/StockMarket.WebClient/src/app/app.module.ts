import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";

import { AppMaterialModule } from "./material.module";
import { AppRouterModule } from "./app.routes";

import { AuthInterceptor } from "@root/_middlewares/auth.interceptor";
import { ErrorInterceptor } from "@root/_middlewares/error.interceptor";

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
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
