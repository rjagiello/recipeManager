import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;
  remindMode = false;

  constructor(private authService: AuthService,
              ) {}

  ngOnInit() {
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  remindToggle() {
    this.remindMode = !this.remindMode;
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }

  cancelRemindMode(remindMode: boolean) {
    this.remindMode = remindMode;
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

}
