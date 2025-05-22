import { Component } from '@angular/core';
import { UserService } from '../../services/userService/user.service';
import { Router } from '@angular/router';
import { InternalCommunicationService } from 'src/app/services/internalCommunicationService/internal-communication.service';

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
    private internalCommunicationService: InternalCommunicationService,
  ) {
    this.login = '';
    this.password = '';
  }

  login: string;
  password: string;
  invalidLogin = false;

  Login(): void {
    this.invalidLogin = false;
    this.loginService.LogIn(this.login, this.password).subscribe({
      next: (result: string) => {
        localStorage.setItem('bearer', JSON.stringify(result));
        this.internalCommunicationService.NewUserLoggedInEvent();
        this.router.navigate(['products']);
      },
      error: () => {
        this.invalidLogin = true;
      },
    });
  }
}
