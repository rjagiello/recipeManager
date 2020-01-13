import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';
import { Product, ProductEdit } from '../_models/product';
import { FridgeService } from '../_services/fridge.service';

@Injectable()
export class FridgeResolver implements Resolve<ProductEdit[]> {

    pageNumber = 1;
    pageSize = 12;

    constructor(private fridgeService: FridgeService,
                private router: Router,
                private alertify: AlertifyService,
                private authService: AuthService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<ProductEdit[]> {
        return this.fridgeService.getFridgeContent(this.authService.decodedToken.nameid,
                                            this.pageNumber,
                                            this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('Problem z pobraniem danych');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
