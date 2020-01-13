import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { ShoppingList } from '../_models/shoppingList';
import { ShoppingService } from '../_services/shopping.service';

@Injectable()
export class ShoppingResolver implements Resolve<ShoppingList[]> {

    constructor(private shoppingService: ShoppingService,
                private router: Router,
                private alertify: AlertifyService,
                private authService: AuthService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<ShoppingList[]> {
        return this.shoppingService.getShoppingLists(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem z pobraniem danych');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
