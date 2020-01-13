import { Component, OnInit } from '@angular/core';
import { Recipe } from '../_models/recipe';
import { User } from '../_models/user';
import { RecipeService } from '../_services/recipe.service';
import { AlertifyService } from '../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginationResult } from '../_models/pagination';
import { RecipeCardEnum } from '../_enums/recipeCard';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.css']
})
export class RecipeComponent implements OnInit {

  recipes: Recipe[];
  pagination: Pagination;
  category = 0;
  user: User = JSON.parse(localStorage.getItem('user'));
  cardType: RecipeCardEnum = RecipeCardEnum.My;
  text: string;
  results: string[];

  constructor(private recipeService: RecipeService,
              private alertify: AlertifyService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.recipes = data.recipes.result;
      this.pagination = data.recipes.pagination;
    });
  }

  loadRecipes() {
    this.recipeService.getRecipesList(this.user.id,
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

  removeRecipe(recipe: Recipe) {
    this.recipes.splice(this.recipes.findIndex(r => r.id === recipe.id), 1);
  }

  search(event) {
    this.recipeService.searchRecipess(this.user.id, event.query).subscribe((data: string[]) => {
      this.results = data;
    }, error => {
      console.log(error);
    });
  }

  getRecipe() {
    this.recipeService.getRecipeResult(this.user.id, this.text).subscribe((data: Recipe[]) => {
      this.recipes = data;
    }, error => {
      this.alertify.error(error);
    });
  }
}
