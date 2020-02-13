import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { HubConnection, IHttpConnectionOptions, HubConnectionBuilder, LogLevel } from '@aspnet/signalr';
import { UserService } from '../_services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  photoUrl: string;
  userName: string;
  isNoti = false;
  hubConnection: HubConnection;
  jwtToken: string;
  baseUrl = environment.signalRUrl;

  constructor(public authService: AuthService,
              private alertify: AlertifyService,
              private router: Router,
              private userService: UserService) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    this.authService.currentUserName.subscribe(userName => this.userName = userName);
    this.authService.currentNoti.subscribe(noti => this.isNoti = noti);
    if (this.loggedIn()) {
      this.connectToHub();
    }
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Zalogowałeś się do aplikacji');
      this.connectToHub();
      this.getInvites();
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['przepisy']);
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logOut() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = null;
    }
    this.alertify.message('Zostałeś wylogowany');
    this.router.navigate(['']);
  }

  getInvites() {
    this.userService.IsInvite(this.authService.decodedToken.nameid).subscribe((res: boolean) => {
      this.isNoti = res;
    }, error => {
      this.alertify.error(error);
    });
  }

  connectToHub() {
    this.jwtToken = localStorage.getItem('token');
    const options: IHttpConnectionOptions = {
      accessTokenFactory: () => {
        return this.jwtToken;
      }
    };

    this.hubConnection = new HubConnectionBuilder()
                        .configureLogging(LogLevel.Information)
                        .withUrl(this.baseUrl, options).build();
    this.hubConnection
    .start()
    .then(() => console.log('Connection started!'))
    .catch(err => console.log(err));

    this.hubConnection.on('BroadcastMessage', (type: string, payload: string) => {
      this.isNoti = true;
      this.alertify.success('<b>' + type + '</b><br />' + payload);
    });
  }
}
