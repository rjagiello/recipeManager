import { Component, OnInit, Input } from '@angular/core';
import { Recipe } from 'src/app/_models/recipe';
import { Pagination, PaginationResult } from 'src/app/_models/pagination';
import { RecipeService } from 'src/app/_services/recipe.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-recipe-friend',
  templateUrl: './recipe-friend.component.html',
  styleUrls: ['./recipe-friend.component.css']
})
export class RecipeFriendComponent implements OnInit {

  @Input() user: User;
  recipes: Recipe[];
  pagination: Pagination;
  category = 0;

  constructor(private recipeService: RecipeService,
              private alertify: AlertifyService,
              private authService: AuthService) { }

  ngOnInit() {
    this.pagination.currentPage = 1;
    this.pagination.itemsPerPage = 8;
    this.pagination.currentPage = 0;
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
}
