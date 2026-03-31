import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserListItem, UserCreateRequest, UserUpdateRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Users`;

  getAll(): Observable<UserListItem[]> {
    return this.http.get<UserListItem[]>(`${this.base}/GetAll`);
  }

  getById(id: string): Observable<UserListItem> {
    return this.http.get<UserListItem>(`${this.base}/GetById/${id}`);
  }

  add(payload: UserCreateRequest): Observable<UserListItem> {
    return this.http.post<UserListItem>(`${this.base}/Add`, payload);
  }

  update(id: string, payload: UserUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/Update/${id}`, payload);
  }
}
