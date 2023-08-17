import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NewUser } from 'src/app/Models/user-registration-form';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  user!: NewUser;
  confirmPassword: string;
  
  constructor(){
    this.user = {
      UserName: '',
      Password: '',
      Name: '',
      Lastname: '',
      Email: '',
      PhonePrefix: '',
      PhoneNumber: '',
      Address: '',
      City: '',
      ZIPCode: '',
      Country: ''
    };
    this.confirmPassword = '';
  }

  


  ngOnInit(): void{
    userForm : new FormGroup({
      UserName: new FormControl(this.user.UserName, [
        Validators.required, Validators.minLength(3)
      ]),
      Password: new FormControl(this.user.Password, [
        Validators.required, Validators.minLength(3),
      ]),
      Name: new FormControl(this.user.Name, [
        Validators.required
      ]),
      Lastname: new FormControl(this.user.Lastname, [
        Validators.required
      ]),
      Email: new FormControl(this.user.Email, [
        Validators.required, Validators.email
      ]),
      PhonePrefix: new FormControl(this.user.PhonePrefix, [
        Validators.required
      ]),
      PhoneNumber: new FormControl(this.user.PhoneNumber, [
        Validators.required
      ]),
      Address: new FormControl(this.user.Address, [
        Validators.required
      ]),
      City: new FormControl(this.user.City, [
        Validators.required
      ]),
      ZIPCode: new FormControl(this.user.ZIPCode, [
        Validators.required
      ]),
      Country: new FormControl(this.user.Country, [
        Validators.required
      ])
    });
  }


  Register(): void{
    console.log(this.user);
  }
}
