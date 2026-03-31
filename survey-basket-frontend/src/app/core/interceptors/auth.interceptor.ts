import {
  HttpInterceptorFn,
  HttpErrorResponse,
  HttpRequest,
  HttpHandlerFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError, BehaviorSubject, filter, take } from 'rxjs';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

const refreshTokenSubject = new BehaviorSubject<string | null>(null);
let isRefreshing = false;

function addBearerToken(req: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
  if (!token) return req;
  return req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const authService = inject(AuthService);
  const router = inject(Router);

  const authedReq = addBearerToken(req, tokenService.accessToken());

  return next(authedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('Auth/Login') && !req.url.includes('Auth/RefreshAuth')) {
        return handleUnauthorized(req, next, tokenService, authService, router);
      }
      return throwError(() => error);
    })
  );
};

function handleUnauthorized(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
  tokenService: TokenService,
  authService: AuthService,
  router: Router
) {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    const refreshToken = tokenService.refreshToken();
    const accessToken = tokenService.accessToken();

    if (!refreshToken || !accessToken) {
      isRefreshing = false;
      tokenService.clearTokens();
      router.navigate(['/auth/login']);
      return throwError(() => new Error('No tokens available'));
    }

    return authService.refreshToken({ token: accessToken, refreshToken }).pipe(
      switchMap((response) => {
        isRefreshing = false;
        tokenService.setTokens(response);
        refreshTokenSubject.next(response.token);
        return next(addBearerToken(req, response.token));
      }),
      catchError((err) => {
        isRefreshing = false;
        tokenService.clearTokens();
        router.navigate(['/auth/login']);
        return throwError(() => err);
      })
    );
  }

  return refreshTokenSubject.pipe(
    filter((token) => token !== null),
    take(1),
    switchMap((token) => next(addBearerToken(req, token)))
  );
}
