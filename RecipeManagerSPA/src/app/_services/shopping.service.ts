import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ShoppingList, ShoppingListToAdd } from '../_models/shoppingList';
import { ShoppingProduct } from '../_models/product';

@Injectable({
  providedIn: 'root'
})
export class ShoppingService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getShoppingLists(userId: number) {
    return this.http.get<ShoppingList[]>(this.baseUrl + 'users/' + userId + '/shopping');
  }

  getAllProducts(userId: number) {
    return this.http.get<ShoppingProduct[]>(this.baseUrl + 'users/' + userId + '/shopping/products');
  }

  addRecipeToShoppingList(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/shopping/' + id, null);
  }

  finishShopping(userId: number, list: ShoppingList) {
    return this.http.put(this.baseUrl + 'users/' + userId + '/shopping', list);
  }

  addShoppingList(userId: number, list: ShoppingListToAdd) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/shopping/', list);
  }

  deleteShoppingList(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/shopping/' + id);
  }

  deleteAllShoppingLists(userId: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/shopping');
  }
}
