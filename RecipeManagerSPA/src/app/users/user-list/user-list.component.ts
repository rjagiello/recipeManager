import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Pagination, PaginationResult } from 'src/app/_models/pagination';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { UserCardEnum } from 'src/app/_enums/userdCard';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {

  users: User[];

  followParam: string;
  pagination: Pagination;
  cardType: UserCardEnum = UserCardEnum.Invite;

  constructor(private userService: UserService,
              private alertify: AlertifyService,
              private route: ActivatedRoute,
              private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data.users.result;
      this.pagination = data.users.pagination;
    });
    this.followParam = 'userFollow';
    this.authService.changeNotifi(false);
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.followParam)
      .subscribe((res: PaginationResult<User[]>) => {
        if (this.followParam === 'userFollow') {
          this.cardType = UserCardEnum.Invite;
        } else {
          this.cardType = UserCardEnum.Friends;
        }
        this.users = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error);
      });
  }

  removeCard(user: User) {
    this.users.splice(this.users.findIndex(u => u.id === user.id), 1);
  }
}
