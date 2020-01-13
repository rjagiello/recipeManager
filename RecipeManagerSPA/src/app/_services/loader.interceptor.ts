import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { LoaderService } from './loader.service';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';


@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
    timer: number;
    constructor(public loaderService: LoaderService) { }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let finished = false;

        setTimeout(() => {
            if (!finished) {
                this.loaderService.show();
            }
        }, 500);

        return next.handle(req).pipe(
            finalize(() => {
                finished = true;
                this.loaderService.hide();
            })
        );
    }
}
