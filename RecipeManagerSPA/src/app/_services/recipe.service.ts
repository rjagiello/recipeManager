import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Recipe, RecipeDetail, RecipeUpdate } from '../_models/recipe';
import { PaginationResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class RecipeService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getRecipesList(userId: number, id: number, page?, itemsPerPage?, category?) {
    const paginationResult: PaginationResult<Recipe[]> = new PaginationResult<Recipe[]>();
    let params = new HttpParams();

    params = params.append('Category', category);
    params = params.append('userId', id.toString());

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<Recipe[]>(this.baseUrl + 'users/' + userId + '/recipes', { observe: 'response', params })
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

  getRecipe(userId: number, id: number) {
    return this.http.get(this.baseUrl + 'users/' + userId + '/recipes/' + id);
  }

  addRecipe(id: number, recipe: RecipeDetail) {
    return this.http.post(this.baseUrl + 'users/' + id + '/recipes/' + 'add', recipe);
  }

  deleteRecipe(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/recipes/' + id);
  }

  updateRecipe(userId: number, id: number, recipe: RecipeUpdate) {
    return this.http.put(this.baseUrl + 'users/' + userId + '/recipes/' + id, recipe);
  }

  searchRecipess(userId: number, term: string) {
    let params = new HttpParams();
    params = params.append('term', term);
    return this.http.get<string[]>(this.baseUrl + 'users/' + userId + '/recipes/search', { observe: 'response', params })
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }

  getRecipeResult(userId: number, name: string) {
    let params = new HttpParams();
    params = params.append('name', name);
    return this.http.get<Recipe[]>(this.baseUrl + 'users/' + userId + '/recipes/search/result', { observe: 'response', params })
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }
}
