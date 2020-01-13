import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginationResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  getUsers(page?, itemsPerPage?, followParam?): Observable<PaginationResult<User[]>> {
    const paginationResult: PaginationResult<User[]> = new PaginationResult<User[]>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (followParam === 'userFollow') {
      params = params.append('userFollow', 'true');
    }

    if (followParam === 'userIsFollowed') {
      params = params.append('userIsFollowed', 'true');
    }

    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
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

  acceptInvite(userId: number, recipientId: number) {
    return this.http.put(this.baseUrl + 'users/' + userId + '/friends/' + recipientId, null );
  }

  sendInvite(userId: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/friends/' + recipientId, null );
  }

  searchUsers(term: string) {
    let params = new HttpParams();
    params = params.append('term', term);
    return this.http.get<string[]>(this.baseUrl + 'users/search', { observe: 'response', params })
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }

  deleteFriend(userId: number, recipientId: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/friends/' + recipientId);
  }

  getUserResult(userName: string) {
    let params = new HttpParams();
    params = params.append('userName', userName);
    return this.http.get<User>(this.baseUrl + 'users/search/result', { observe: 'response', params })
    .pipe(
      map(response => {
        return response.body;
      })
    );
  }

  IsInvite(userId: number) {
    return this.http.get<boolean>(this.baseUrl + 'users/invite/' + userId);
  }
}
