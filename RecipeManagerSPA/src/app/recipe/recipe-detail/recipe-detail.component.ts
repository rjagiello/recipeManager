import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RecipeDetail } from 'src/app/_models/recipe';
import { ShoppingService } from 'src/app/_services/shopping.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { RecipeCardEnum } from 'src/app/_enums/recipeCard';
import { RecipeService } from 'src/app/_services/recipe.service';

@Component({
  selector: 'app-recipe-detail',
  templateUrl: './recipe-detail.component.html',
  styleUrls: ['./recipe-detail.component.css']
})
export class RecipeDetailComponent implements OnInit {

  recipe: RecipeDetail;
  cardType: RecipeCardEnum;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private shoppingService: ShoppingService,
              private authService: AuthService,
              private alertify: AlertifyService,
              private recipeService: RecipeService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.recipe = data.recipe;
      this.cardType = RecipeCardEnum[this.route.snapshot.paramMap.get('type')];
    });
  }

  addRecipeToShoppingList() {
    this.shoppingService.addRecipeToShoppingList(this.authService.decodedToken.nameid, this.recipe.id).subscribe(() => {
      this.alertify.success('Dodano przepis do listy zakupów');
    }, error => {
      this.alertify.error(error);
    });
  }

  deleteRecipe() {
    this.alertify.confirm('Czy na pewno chcesz usunąć przepis?', () => {
      this.recipeService.deleteRecipe(this.authService.decodedToken.nameid, this.recipe.id).subscribe(() => {
        this.router.navigate(['/przepisy']);
        this.alertify.success('Przepis został usunięty');
      }, error => {
        this.alertify.error('Nie udało się usunąć przepisu');
      });
    });
  }

  addToMyRecipes() {
    this.recipe.id = 0;
    this.recipeService.addRecipe(this.authService.decodedToken.nameid, this.recipe).subscribe(() => {
      this.alertify.success('Dodano do przepisów');
    }, error => {
      this.alertify.error(error);
    });
  }

  isMyType() {
    return this.cardType === RecipeCardEnum.My;
  }

  isFriendType() {
    return this.cardType === RecipeCardEnum.Friend;
  }

}
