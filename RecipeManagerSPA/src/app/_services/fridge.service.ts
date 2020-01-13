import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Product, ProductEdit } from '../_models/product';
import { PaginationResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FridgeService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getFridgeContent(id: number, page?, itemsPerPage?) {
    const paginationResult: PaginationResult<ProductEdit[]> = new PaginationResult<ProductEdit[]>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<ProductEdit[]>(this.baseUrl + 'users/' + id + '/fridge', { observe: 'response', params })
    .pipe(
      map(response => {
        paginationResult.result = response.body;

        if (response.headers.get('Pagination') != null) {
          paginationResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }

        return paginationResult;
      })
    );
  }

  searchProducts(userId: number, term: string) {
    let params = new HttpParams();
    params = params.append('term', term);
    return this.http.get<string[]>(this.baseUrl + 'users/' + userId + '/fridge/search', { observe: 'response', params })
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }

  getProductResult(userId: number, name: string) {
    let params = new HttpParams();
    params = params.append('name', name);
    return this.http.get<ProductEdit>(this.baseUrl + 'users/' + userId + '/fridge/search/result', { observe: 'response', params })
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }

  addProduct(userId: number, product: ProductEdit) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/fridge', product);
  }

  deleteProduct(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/fridge/' + id);
  }

  updateProduct(userId: number, id: number, product: ProductEdit) {
    return this.http.put(this.baseUrl + 'users/' + userId + '/fridge/' + id, product);
  }
}

