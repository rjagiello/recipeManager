import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { RecipeDetail } from '../_models/recipe';
import { RecipeService } from '../_services/recipe.service';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class RecipeDetailResolver implements Resolve<RecipeDetail> {

    constructor(private recipeService: RecipeService,
                private router: Router,
                private alertify: AlertifyService,
                private authService: AuthService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<RecipeDetail> {
        return this.recipeService.getRecipe(this.authService.decodedToken.nameid, route.params.id).pipe(
            catchError(error => {
                this.alertify.error('Problem z pobraniem danych');
                this.router.navigate(['/przepisy']);
                return of(null);
            })
        );
    }
}
