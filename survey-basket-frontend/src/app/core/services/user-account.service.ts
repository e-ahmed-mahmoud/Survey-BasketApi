import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AccountInfo, UpdateAccountRequest, ChangePasswordRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class UserAccountService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/UserAccount`;

  getAccountInfo(): Observable<AccountInfo> {
    return this.http.get<AccountInfo>(`${this.base}/GetAccountInfo`);
  }

  updateAccount(payload: UpdateAccountRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/UpdateUserAccount`, payload);
  }

  changePassword(payload: ChangePasswordRequest): Observable<void> {
    return this.http.put<void>(`${this.base}/ChangeUserPassword`, payload);
  }
}
