import { Injectable } from "@angular/core";
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor
} from "@angular/common/http";
import { Observable } from "rxjs";

import { AuthenticationService } from "@root/_services/auth.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private _authService: AuthenticationService;

    constructor(authService: AuthenticationService) {
        this._authService = authService;
    }

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        const request_clone = request.clone();
        const currentUser = this._authService.currentUser;
        if (currentUser && currentUser.accessToken) {
            const tokenString = "Basic " + currentUser.accessToken;
            request_clone.headers.set("Authorization", tokenString);
        }
        return next.handle(request_clone);
    }
}
