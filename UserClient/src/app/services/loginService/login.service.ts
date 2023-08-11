import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { UserCredentials } from 'src/app/Models/user-credentials';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private httpClient: HttpClient) { }

  LogIn(login: string, password: string): Observable<any> {
    const user = new UserCredentials();
    user.Login = "alice";
    user.Password = "user123";
    const options = { withCredentials: true
    };

    
    return this.httpClient.post<Observable<any> >(
      'http://localhost:5156/api/v1/User/login', user, options);
  }

  TestRequest(): Observable<any> {
    return this.httpClient.get<Observable<any> >(
      'http://localhost:5286/weatherforecast', { withCredentials: true}
    );
  }

}
