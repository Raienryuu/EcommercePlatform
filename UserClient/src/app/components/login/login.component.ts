import { Component } from '@angular/core';
import { UserService } from '../../services/userService/user.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {

  constructor(private loginService: UserService) {
    this.login = '';
    this.password = '';
  }

  login: string;
  password: string;

  Login(): void {
    this.loginService.LogIn(this.login, this.password).subscribe(
      (result: unknown) => console.log(result));
  }


}
