import { Injectable } from "@angular/core";
import {
    Router,
    CanActivate,
    ActivatedRouteSnapshot,
    RouterStateSnapshot
} from "@angular/router";

import { AuthenticationService } from "@root/_services/auth.service";

@Injectable({ providedIn: "root" })
export class AuthGuard implements CanActivate {
    private _router: Router;
    private _authService: AuthenticationService;

    constructor(router: Router, authService: AuthenticationService) {
        this._router = router;
        this._authService = authService;
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        // logged in can access everything
        const currentUser = this._authService.currentUser;
        if (currentUser !== null) {
            return true;
        }

        this._router.navigateByUrl("/");
        return false;
    }
}
