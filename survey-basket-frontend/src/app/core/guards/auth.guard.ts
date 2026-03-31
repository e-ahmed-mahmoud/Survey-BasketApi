import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.isLoggedIn()) {
    return true;
  }

  router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
  return false;
};

export const noAuthGuard: CanActivateFn = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (!tokenService.isLoggedIn()) {
    return true;
  }

  router.navigate(['/dashboard']);
  return false;
};

export const roleGuard = (requiredRole: string): CanActivateFn =>
  () => {
    const tokenService = inject(TokenService);
    const router = inject(Router);

    if (tokenService.hasRole(requiredRole)) {
      return true;
    }

    router.navigate(['/unauthorized']);
    return false;
  };
