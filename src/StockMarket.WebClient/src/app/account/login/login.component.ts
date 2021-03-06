import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { FormGroup, FormBuilder } from "@angular/forms";
import { AbstractControl } from "@angular/forms";
import { Subject, of } from "rxjs";
import { catchError, finalize, takeUntil, tap } from "rxjs/operators";

import { AuthenticationService } from "@root/_services/auth.service";
import { AuthUser } from "@root/_models/authuser";

@Component({
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit, OnDestroy {
    private _formBuilder: FormBuilder;
    private _router: Router;
    private _authService: AuthenticationService;

    private destroy$ = new Subject<void>();

    public loginForm?: FormGroup;
    public formError = "";
    public formLoading = false;

    get username(): AbstractControl | null {
        return this.loginForm?.get("username")!;
    }
    get password(): AbstractControl | null {
        return this.loginForm?.get("password")!;
    }

    constructor(
        formBuilder: FormBuilder,
        router: Router,
        authService: AuthenticationService
    ) {
        this._formBuilder = formBuilder;
        this._router = router;
        this._authService = authService;
    }

    ngOnInit(): void {
        if (this._authService.currentUser != null) {
            this._router.navigateByUrl("/dashboard");
        }

        this.loginForm = this._formBuilder.group({
            username: [""],
            password: [""]
        });
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    onSubmit(): void {
        if (this.loginForm!.invalid) return;
        this.formLoading = true;
        this._authService
            .login(this.username!.value, this.password!.value)
            .pipe(takeUntil(this.destroy$))
            .pipe(tap(() => this.handleLoginSuccess))
            .pipe(catchError((err) => of(this.handleLoginError(err))))
            .pipe(finalize(() => (this.formLoading = false)))
            .subscribe();
    }

    private handleLoginSuccess() {
        this._router.navigateByUrl("/dashboard");
    }

    private handleLoginError(error: any): void {
        if (error.status === 400) {
            this.formError = "Incorrect username or password";
        } else {
            this.formError = "Unknown error. Please try again";
        }
    }
}
