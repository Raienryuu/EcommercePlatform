import { Component } from '@angular/core';
import { UserService } from '../../services/userService/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  constructor(
    private loginService: UserService,
    private router: Router,
  ) {
    this.login = '';
    this.password = '';
  }

  login: string;
  password: string;

  Login(): void {
    this.loginService
      .LogIn(this.login, this.password)
      .subscribe((result: string) => {
        localStorage.setItem('bearer', JSON.stringify(result));
        this.router.navigate(['products']);
      });
  }
}
