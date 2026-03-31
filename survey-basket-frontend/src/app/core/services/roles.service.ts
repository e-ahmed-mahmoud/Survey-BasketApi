import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RoleItem, RoleRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class RolesService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/Roles`;

  getAll(): Observable<RoleItem[]> {
    return this.http.get<RoleItem[]>(`${this.base}/GetAll`);
  }

  getById(id: string): Observable<RoleItem> {
    return this.http.get<RoleItem>(`${this.base}/GetById/${id}`);
  }

  add(payload: RoleRequest): Observable<RoleItem> {
    return this.http.post<RoleItem>(`${this.base}/Add`, payload);
  }

  update(id: string, payload: RoleRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/Update/${id}`, payload);
  }
}
