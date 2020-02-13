import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute, Router } from '@angular/router';
import { Recipe } from 'src/app/_models/recipe';
import { Pagination, PaginationResult } from 'src/app/_models/pagination';
import { RecipeService } from 'src/app/_services/recipe.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { RecipeCardEnum } from 'src/app/_enums/recipeCard';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {

  user: User;
  recipes: Recipe[];
  pagination: any = {};
  category: number;
  cardType: RecipeCardEnum = RecipeCardEnum.Friend;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private recipeService: RecipeService,
              private alertify: AlertifyService,
              private authService: AuthService,
              private userService: UserService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data.user;
    }),
    this.pagination.currentPage = 1;
    this.pagination.itemsPerPage = 6;
    this.category = 0;
    this.loadRecipes();
  }

  loadRecipes() {
    this.recipeService.getRecipesList(
      this.authService.decodedToken.nameid,
      this.user.id,
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.category)
      .subscribe((res: PaginationResult<Recipe[]>) => {
        this.recipes = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error);
      });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadRecipes();
  }

  deleteFriendShip() {
    this.alertify.confirm('Czy na pewno usunąć ze znajomych użytkownika ' + this.user.userName + '?', () => {
      this.userService.deleteFriend(this.authService.decodedToken.nameid, this.user.id).subscribe(() => {
        this.alertify.warning('Usunięto ze znajomych użytkownika ' + this.user.userName);
        this.router.navigate(['/znajomi']);
      }, error => {
        this.alertify.error(error);
      });
    });
  }

}
