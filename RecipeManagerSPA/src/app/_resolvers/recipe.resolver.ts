import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { Recipe } from '../_models/recipe';
import { RecipeService } from '../_services/recipe.service';

@Injectable()
export class RecipesResolver implements Resolve<Recipe[]> {

    pageNumber = 1;
    pageSize = 8;
    category = 0;

    constructor(private recipeService: RecipeService,
                private router: Router,
                private alertify: AlertifyService,
                private authService: AuthService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<Recipe[]> {
        return this.recipeService.getRecipesList(this.authService.decodedToken.nameid,
                                            this.authService.decodedToken.nameid,
                                            this.pageNumber,
                                            this.pageSize,
                                            this.category).pipe(
            catchError(error => {
                this.alertify.error('Problem z pobraniem danych');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
