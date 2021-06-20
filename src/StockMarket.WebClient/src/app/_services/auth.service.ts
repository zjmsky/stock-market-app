import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { v4 as uuidv4 } from "uuid";

import { environment } from "@env/environment";
import { AuthUser } from "@root/_models/authuser";

@Injectable({ providedIn: "root" })
export class AuthenticationService {
    private _currentUserSubject: BehaviorSubject<AuthUser | null>;
    private _http: HttpClient;
    private _deviceId: string;
    private _url: string;

    public get currentUser(): AuthUser | null {
        return this._currentUserSubject.value;
    }

    constructor(http: HttpClient) {
        const userItem = JSON.parse(localStorage.getItem("currentUser")!);
        this._currentUserSubject = new BehaviorSubject(userItem);
        this._http = http;
        this._deviceId = this.getDeviceId();
        this._url = environment.apiUrl;
    }

    public register(
        username: string,
        password: string,
        email: string
    ): Observable<void> {
        const registerUrl = `${this._url}/auth/register`;
        const registerBody = { username, password, email };
        return this._http.post<void>(registerUrl, registerBody);
    }

    public login(username: string, password: string): Observable<AuthUser> {
        const loginUrl = `${this._url}/auth/login`;
        const loginBody = { username, password, deviceId: this._deviceId };
        return this._http.post<AuthUser>(loginUrl, loginBody).pipe(
            tap((user) => {
                localStorage.setItem("currentUser", JSON.stringify(user));
                this._currentUserSubject.next(user);
            })
        );
    }

    public refresh(): Observable<object> {
        const userObject = JSON.parse(localStorage.getItem("currentUser")!);
        const refreshToken = userObject.refreshToken;
        const refreshUrl = `${this._url}/auth/refresh`;
        const refreshBody = { refreshToken, deviceId: this._deviceId };
        return this._http.post<AuthUser>(refreshUrl, refreshBody).pipe(
            tap((user) => {
                localStorage.setItem("currentUser", JSON.stringify(user));
                this._currentUserSubject.next(user);
            })
        );
    }

    public logout(): Observable<object> {
        const userObject = JSON.parse(localStorage.getItem("currentUser")!);
        const refreshToken = userObject.refreshToken;
        const logoutUrl = `${this._url}/auth/logout`;
        const logoutBody = { refreshToken, deviceId: this._deviceId };
        return this._http.post(logoutUrl, logoutBody).pipe(
            tap(() => {
                localStorage.removeItem("currentUser");
                this._currentUserSubject.next(null);
            })
        );
    }

    private getDeviceId(): string {
        const deviceId = localStorage.getItem("deviceId");
        if (deviceId !== null) return deviceId;

        const newId = uuidv4();
        localStorage.setItem("deviceId", newId);
        return newId;
    }
}
