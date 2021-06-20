import { Injectable } from "@angular/core";
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor
} from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

import { AuthenticationService } from "@root/_services/auth.service";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    private _authService: AuthenticationService;

    constructor(authService: AuthenticationService) {
        this._authService = authService;
    }

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        return next.handle(request)
            .pipe(catchError((err) => this.handleError(err)));
    }

    private handleError(error: any) {
        if (error.status == 401) {
            this._authService.logout();
            location.reload();
        }
        return throwError(error);
    }
}
