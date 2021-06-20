import { Component } from "@angular/core";

import { AuthUser } from "./_models/authuser";
import { AuthenticationService } from "./_services/auth.service";

@Component({
    selector: "app-root",
    templateUrl: "./app.component.html"
})
export class AppComponent {
    title = "stock-analyzer";

    private _authService: AuthenticationService;

    constructor(authService: AuthenticationService) {
        this._authService = authService;
    }

    get currentUser(): AuthUser | null {
        return this._authService.currentUser;
    }
}
