import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from './_services/auth.service';
import { User } from './_models/user';
import { WindowRef } from './_services/window.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {

  jwtHelper = new JwtHelperService();
  isChrome = true;
  isIE = false;

  constructor(private authService: AuthService,
              private window: WindowRef) {}

  ngOnInit(): void {
    this.isIE = /msie\s|trident\/|edge\//i.test(window.navigator.userAgent);
    this.isChrome = !!this.window.nativeWindow.chrome &&
    (!!this.window.nativeWindow.chrome.webstore || !!this.window.nativeWindow.chrome.runtime);
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      this.authService.changeUserPhoto(user.photoUrl);
      this.authService.changeUserName(user.userName);
    }
    localStorage.setItem('noti', 'false');
  }
}
