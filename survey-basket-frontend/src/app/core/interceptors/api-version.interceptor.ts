import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../../environments/environment';

export const apiVersionInterceptor: HttpInterceptorFn = (req, next) => {
  // Skip external URLs
  if (!req.url.startsWith(environment.apiUrl) && !req.url.startsWith('/api')) {
    return next(req);
  }

  const versionedReq = req.clone({
    setHeaders: {
      'x-api-version': environment.apiVersion,
    },
  });

  return next(versionedReq);
};
