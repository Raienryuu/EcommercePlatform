import { Component } from '@angular/core';
import { UserSettingsService } from 'src/app/services/userSettingsService/user-settings.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  /**
   *
   */
  constructor(private userSettingsService: UserSettingsService) {
    this.userSettingsService.GetUserName().subscribe((name) => {
      this.Name = name;
    });
  }
  Name = '';
}
