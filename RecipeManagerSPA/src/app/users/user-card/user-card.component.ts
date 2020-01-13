import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserCardEnum } from 'src/app/_enums/userdCard';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.css']
})
export class UserCardComponent implements OnInit {

  @Input() user: User;
  @Output() removeCard = new EventEmitter<User>();
  @Input() cardType: UserCardEnum;

  constructor(private authService: AuthService,
              private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
  }

  acceptInvite() {
    this.userService.acceptInvite(this.authService.decodedToken.nameid, this.user.id).subscribe(() => {
      this.removeCard.emit(this.user);
      this.alertify.success('Zaakceptowano zaproszenie od użytkownkika ' + this.user.userName);
    }, error => {
      this.alertify.error(error);
    });
  }

  deleteFriend() {
    this.alertify.confirm('Czy na pewno usunąć zaproszenie?', () => {
      this.userService.deleteFriend(this.authService.decodedToken.nameid, this.user.id).subscribe(() => {
        this.removeCard.emit(this.user);
        this.alertify.warning('Usunięto zaproszenie od użytkownika ' + this.user.userName);
      }, error => {
        this.alertify.error(error);
      });
    });
  }

  sendInvite() {
    this.userService.sendInvite(this.authService.decodedToken.nameid, this.user.id).subscribe(() => {
      this.alertify.success('Wysłano zaproszenie do użytkownika ' + this.user.userName);
    }, error => {
      this.alertify.error(error);
    });
  }

  isInviteType() {
    return this.cardType === UserCardEnum.Invite;
  }

  isFriendsType() {
    return this.cardType === UserCardEnum.Friends;
  }

  isSearchType() {
    return this.cardType === UserCardEnum.Search;
  }

}
