import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserCardEnum } from 'src/app/_enums/userdCard';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.css']
})
export class UserSearchComponent implements OnInit {

  user: User;
  text: string;
  results: string[];
  cardType: UserCardEnum;

  constructor(private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.cardType = UserCardEnum.Search;
  }

  search(event) {
    this.userService.searchUsers(event.query).subscribe((data: string[]) => {
      this.results = data;
    }, error => {
      console.log(error);
    });
  }

  getUser() {
    this.userService.getUserResult(this.text).subscribe((data: User) => {
      this.user = data;
    }, error => {
      this.alertify.error(error);
    });
  }
}
