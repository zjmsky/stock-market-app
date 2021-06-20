import { Component } from "@angular/core";
import { tap } from "rxjs/operators";

import { AuthUser } from "@root/_models/authuser";
import { AuthenticationService } from "@root/_services/auth.service";

@Component({
    selector: "app-toolbar",
    templateUrl: "./toolbar.component.html",
    styleUrls: ["./toolbar.component.css"]
})
export class ToolbarComponent {
    private _authService: AuthenticationService;

    constructor(authService: AuthenticationService) {
        this._authService = authService;
    }

    get currentUser(): AuthUser | null {
        return this._authService.currentUser;
    }

    onLogoutClick() {
        this._authService
            .logout()
            .pipe(
                tap(
                    (response) => location.reload(),
                    (response) => location.reload()
                )
            )
            .subscribe();
    }
}
