import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable } from "rxjs";
import { tap } from "rxjs/operators";

import { environment } from "@env/environment";
import { AuthUser } from "@root/_models/authuser";

@Injectable({ providedIn: "root" })
export class AuthenticationService {
    private _currentUserSubject: BehaviorSubject<AuthUser | null>;
    private _http: HttpClient;

    public get currentUser(): AuthUser | null {
        return this._currentUserSubject.value;
    }

    constructor(http: HttpClient) {
        const userItem = JSON.parse(localStorage.getItem("currentUser")!);
        this._currentUserSubject = new BehaviorSubject(userItem);
        this._http = http;
    }

    public login(username: string, password: string): Observable<AuthUser> {
        const loginUrl = environment.apiUrl + "/auth/login";
        const loginBody = { username, password };
        return this._http.post<AuthUser>(loginUrl, loginBody).pipe(
            tap((user) => {
                localStorage.setItem("currentUser", JSON.stringify(user));
                this._currentUserSubject.next(user);
            })
        );
    }

    public refresh(): Observable<object> {
        const refreshUrl = environment.apiUrl + "/auth/refresh";
        const refreshBody = JSON.parse(localStorage.getItem("currentUser")!);
        return this._http.post<AuthUser>(refreshUrl, refreshBody).pipe(
            tap((user) => {
                localStorage.setItem("currentUser", JSON.stringify(user));
                this._currentUserSubject.next(user);
            })
        );
    }

    public logout(): Observable<object> {
        const logoutUrl = environment.apiUrl + "/auth/logout";
        const logoutBody = JSON.parse(localStorage.getItem("currentUser")!);
        return this._http.post(logoutUrl, logoutBody).pipe(
            tap(() => {
                localStorage.removeItem("currentUser");
                this._currentUserSubject.next(null);
            })
        );
    }
}
