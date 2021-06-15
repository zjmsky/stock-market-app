import { Component } from "@angular/core";
import { FormGroup, FormControl, Validators } from "@angular/forms";
import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

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

const registerForm = new FormGroup({
    email: new FormControl("", [Validators.required, Validators.email]),
    username: new FormControl("", UsernameValidator.username(6, 20)),
    password: new FormControl("", PasswordValidator.password(8, 20)),
    passwordConfirm: new FormControl(
        "",
        ConfirmValidator.confirmField("password")
    )
});

registerForm.controls.password.valueChanges.subscribe(() => {
    registerForm.controls.passwordConfirm.updateValueAndValidity();
});

@Component({
    selector: "app-register",
    templateUrl: "./register.component.html",
    styleUrls: ["./register.component.css"]
})
export class RegisterComponent {
    registerForm = registerForm;

    public get email() {
        return this.registerForm.get("email") as AbstractControl;
    }
    get username() {
        return this.registerForm.get("username") as AbstractControl;
    }
    get password() {
        return this.registerForm.get("password") as AbstractControl;
    }
    get passwordConfirm() {
        return this.registerForm.get("passwordConfirm") as AbstractControl;
    }
}
