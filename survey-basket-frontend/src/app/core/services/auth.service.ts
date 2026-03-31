import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';
import {
  AuthRequest,
  AuthResponse,
  RefreshTokenRequest,
  RegisterRequest,
  EmailConfirmRequest,
  ResendConfirmationRequest,
  ResetPasswordRequest,
  AccountInfo,
  UpdateAccountRequest,
  ChangePasswordRequest,
} from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly base = `${environment.apiUrl}/api/Auth`;

  readonly isLoggedIn = this.tokenService.isLoggedIn;
  readonly currentUserRoles = computed(() => this.tokenService.getUserRoles());

  login(credentials: AuthRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/Login`, credentials).pipe(
      tap((response) => {
        console.log(response);
        this.tokenService.setTokens(response)

      })
    );
  }

  register(payload: RegisterRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/Register`, payload);
  }

  refreshToken(payload: RefreshTokenRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/RefreshAuth`, payload).pipe(
      tap((response) => this.tokenService.setTokens(response))
    );
  }

  confirmEmail(payload: EmailConfirmRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/ConfirmEmail`, payload);
  }

  resendConfirmationEmail(payload: ResendConfirmationRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/ResendConfirmEmail`, payload);
  }

  forgotPassword(email: string): Observable<void> {
    const params = new HttpParams().set('Email', email);
    return this.http.post<void>(`${this.base}/ForgetUserPassword`, null, { params });
  }

  resetPassword(payload: ResetPasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.base}/ResetPassword`, payload);
  }

  logout(): void {
    this.tokenService.clearTokens();
    this.router.navigate(['/auth/login']);
  }

  hasRole(role: string): boolean {
    console.log(this.tokenService.hasRole(role));
    console.log(this.tokenService.getUserRoles());
    return this.tokenService.hasRole(role);
  }
}
