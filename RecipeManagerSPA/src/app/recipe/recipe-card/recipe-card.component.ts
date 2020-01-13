import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Recipe } from 'src/app/_models/recipe';
import { RecipeService } from 'src/app/_services/recipe.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { RecipeCardEnum } from 'src/app/_enums/recipeCard';
import { Router } from '@angular/router';
import { ShoppingService } from 'src/app/_services/shopping.service';

@Component({
  selector: 'app-recipe-card',
  templateUrl: './recipe-card.component.html',
  styleUrls: ['./recipe-card.component.css']
})
export class RecipeCardComponent implements OnInit {

  @Input() recipe: Recipe;
  @Output() removeRecipe = new EventEmitter<Recipe>();
  @Input() cardType: RecipeCardEnum;

  constructor(private recipeService: RecipeService,
              private shoppingService: ShoppingService,
              private authService: AuthService,
              private alertify: AlertifyService,
              private router: Router) { }

  ngOnInit() {
  }

  deleteRecipe() {
    this.alertify.confirm('Czy na pewno chcesz usunąć przepis?', () => {
      this.recipeService.deleteRecipe(this.authService.decodedToken.nameid, this.recipe.id).subscribe(() => {
        this.removeRecipe.emit(this.recipe);
        this.alertify.success('Przepis został usunięty');
      }, error => {
        this.alertify.error('Nie udało się usunąć przepisu');
      });
    });
  }

  addRecipeToShoppingList() {
    this.shoppingService.addRecipeToShoppingList(this.authService.decodedToken.nameid, this.recipe.id).subscribe(() => {
      this.alertify.success('Dodano przepis do listy zakupów');
    }, error => {
      this.alertify.error(error);
    });
  }

  onRoute() {
    this.router.navigate(['/przepisy/szczegoly/' + this.recipe.id + '/' + RecipeCardEnum[this.cardType]]);
  }

  isMyType() {
    return this.cardType === RecipeCardEnum.My;
  }

  isFriendType() {
    return this.cardType === RecipeCardEnum.Friend;
  }
}
