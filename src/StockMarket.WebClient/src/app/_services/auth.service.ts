import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, of } from "rxjs";
import { map } from "rxjs/operators";
import { v4 as uuidv4 } from "uuid";

import { environment } from "@env/environment";
import { AuthUser } from "@root/_models/authuser";

@Injectable({ providedIn: "root" })
export class AuthenticationService {
    private _http: HttpClient;
    private _url: string;
    private _currentUserSubject: BehaviorSubject<AuthUser | null>;

    public get currentUser(): AuthUser | null {
        return this._currentUserSubject.value;
    }

    constructor(http: HttpClient) {
        const userItem = JSON.parse(localStorage.getItem("currentUser")!);
        this._currentUserSubject = new BehaviorSubject(userItem);
        this._http = http;
        this._url = environment.apiUrl;
    }

    private getDeviceId(): string {
        const deviceId = localStorage.getItem("deviceId");
        if (deviceId !== null) return deviceId;

        const newId = uuidv4();
        localStorage.setItem("deviceId", newId);
        return newId;
    }

    private handleLogin(user: AuthUser): void {
        localStorage.setItem("currentUser", JSON.stringify(user));
        this._currentUserSubject.next(user);
    }

    private handleLogout(): void {
        localStorage.removeItem("currentUser");
        this._currentUserSubject.next(null);
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

    public login(username: string, password: string): Observable<void> {
        const deviceId = this.getDeviceId();
        const loginUrl = `${this._url}/auth/login`;
        const loginBody = { username, password, deviceId };
        return this._http
            .post<AuthUser>(loginUrl, loginBody)
            .pipe(map(this.handleLogin.bind(this)));
    }

    public refresh(): Observable<void> {
        const userObject = JSON.parse(localStorage.getItem("currentUser")!);
        const deviceId = this.getDeviceId();
        const refreshToken = userObject.refreshToken;
        const refreshUrl = `${this._url}/auth/refresh`;
        const refreshBody = { refreshToken, deviceId };
        return this._http
            .post<AuthUser>(refreshUrl, refreshBody)
            .pipe(map(this.handleLogin.bind(this)));
    }

    public logout(): Observable<void> {
        const userObject = JSON.parse(localStorage.getItem("currentUser")!);
        const deviceId = this.getDeviceId();
        const refreshToken = userObject.refreshToken;
        const logoutUrl = `${this._url}/auth/logout`;
        const logoutBody = { refreshToken, deviceId };
        return this._http
            .post<void>(logoutUrl, logoutBody)
            .pipe(map(this.handleLogout.bind(this)));
    }
}
