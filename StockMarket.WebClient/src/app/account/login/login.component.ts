import { Component } from "@angular/core";
import { FormGroup, FormControl } from "@angular/forms";

const loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
});

@Component({
    selector: "app-login",
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.css"]
})
export class LoginComponent {
    loginForm = loginForm;
}
