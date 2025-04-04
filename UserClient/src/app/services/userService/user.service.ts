import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UserCredentials } from 'src/app/models/user-credentials';
import { NewUser } from 'src/app/models/user-registration-form';
import { environment } from 'src/enviroment';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private httpClient: HttpClient) {}
  options = { withCredentials: true };

  LogIn(login: string, password: string): Observable<never> {
    const user = new UserCredentials();
    user.Login = login;
    user.Password = password;

    return this.httpClient.post<never>(
      environment.apiUrl + 'v1/user/login',
      user,
      this.options,
    );
  }

  Register(user: NewUser): Observable<never> {
    return this.httpClient.post<never>(
      environment.apiUrl + 'v1/user/register',
      user,
      this.options,
    );
  }
}
