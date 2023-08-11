import { Component } from '@angular/core';
import { LoginService } from '../../services/loginService/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {

    constructor(private loginService: LoginService){
      this.login = '';
      this.password = '';
    }

    login : string;
    password : string;

    Login(): void{
      this.loginService.LogIn(this.login, this.password).subscribe(
        (result: any) => console.log(result));
    }

    Test(): void {
      this.loginService.TestRequest()
      .subscribe((result: any) => console.log(result));
    }
}
