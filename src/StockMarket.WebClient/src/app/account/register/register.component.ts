import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { Subject, of } from "rxjs";
import { catchError, finalize, first, takeUntil, tap } from "rxjs/operators";

import { AuthenticationService } from "@root/_services/auth.service";

class UsernameValidator {
    private static usernameInner(
        minLength: number,
        maxLength: number,
        control: AbstractControl
    ): ValidationErrors | null {
        const value = control.value as string;

        if (value.length == 0) return { required: true };
        if (value.length < minLength) return { minlength: true };
        if (value.length > maxLength) return { maxlength: true };

        if (!/^\w+$/.test(value)) return { username: true };

        return null;
    }

    public static username(minLength: number, maxLength: number): ValidatorFn {
        return (control) => this.usernameInner(minLength, maxLength, control);
    }
}

class PasswordValidator {
    private static passwordInner(
        minLength: number,
        maxLength: number,
        control: AbstractControl
    ): ValidationErrors | null {
        const value = control.value as string;

        if (value.length == 0) return { required: true };
        if (value.length < minLength) return { minlength: true };
        if (value.length > maxLength) return { maxlength: true };

        if (!/[a-z]/.test(value)) return { password: true };
        if (!/[A-Z]/.test(value)) return { password: true };
        if (!/\d/.test(value)) return { password: true };
        if (!/\W/.test(value)) return { password: true };

        return null;
    }

    public static password(minLength: number, maxLength: number): ValidatorFn {
        return (control) => this.passwordInner(minLength, maxLength, control);
    }
}

class ConfirmValidator {
    private static confirmFieldInner(
        field: string,
        control: AbstractControl
    ): ValidationErrors | null {
        if (control.parent == null) return { match: true };
        if (control.parent.value === null) return { match: true };

        type ControlSet = { [key: string]: AbstractControl };
        const controls = control.parent.controls as ControlSet;
        if (control.value !== controls[field].value) return { match: true };

        return null;
    }

    public static confirmField(field: string): ValidatorFn {
        return (control) => this.confirmFieldInner(field, control);
    }
}

@Component({
    templateUrl: "./register.component.html",
    styleUrls: ["./register.component.css"]
})
export class RegisterComponent implements OnInit, OnDestroy {
    private _formBuilder: FormBuilder;
    private _router: Router;
    private _authService: AuthenticationService;

    private destroy$ = new Subject<void>();

    public registerForm?: FormGroup;
    public formError = "";
    public formLoading = false;

    get email(): AbstractControl | null {
        return this.registerForm?.get("email")!;
    }
    get username(): AbstractControl | null {
        return this.registerForm?.get("username")!;
    }
    get password(): AbstractControl | null {
        return this.registerForm?.get("password")!;
    }
    get passwordConfirm(): AbstractControl | null {
        return this.registerForm?.get("passwordConfirm")!;
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

        this.registerForm = this._formBuilder.group({
            email: ["", [Validators.required, Validators.email]],
            username: ["", UsernameValidator.username(6, 20)],
            password: ["", PasswordValidator.password(8, 20)],
            passwordConfirm: ["", ConfirmValidator.confirmField("password")]
        });

        const confirmField = this.registerForm.controls.passwordConfirm;
        this.registerForm.controls.password.valueChanges
            .pipe(takeUntil(this.destroy$))
            .pipe(tap(() => confirmField.updateValueAndValidity()))
            .subscribe();
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    onSubmit(): void {
        if (this.registerForm!.invalid) return;
        this.formLoading = true;
        this._authService
            .register(
                this.username!.value,
                this.password!.value,
                this.email!.value
            )
            .pipe(takeUntil(this.destroy$))
            .pipe(first())
            .pipe(tap(() => this.handleRegisterSuccess()))
            .pipe(catchError((err) => of(this.handleRegisterFailure(err))))
            .pipe(finalize(() => (this.formLoading = false)))
            .subscribe();
    }

    private handleRegisterSuccess() {
        this._router.navigateByUrl("/account/login");
    }

    private handleRegisterFailure(error: any) {
        if (error.status === 400) {
            this.formError = "Validation error";
        } else {
            this.formError = "Unknown error. Please try again";
        }
    }
}
