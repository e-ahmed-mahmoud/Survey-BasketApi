import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'An unexpected error occurred.';

      if (error.status === 0) {
        message = 'Cannot connect to server. Please check your connection.';
      } else if (error.status === 400) {
        // Validation errors
        const errors = error.error?.errors;
        if (errors) {
          const firstError = Object.values(errors).flat()[0];
          message = firstError ? String(firstError) : 'Validation failed.';
        } else {
          message = error.error?.title || error.error?.detail || 'Bad request.';
        }
      } else if (error.status === 401) {
        message = 'Your session has expired. Please login again.';
      } else if (error.status === 403) {
        message = 'You do not have permission to perform this action.';
      } else if (error.status === 404) {
        message = 'The requested resource was not found.';
      } else if (error.status === 409) {
        message = error.error?.detail || 'Conflict: The resource already exists.';
      } else if (error.status >= 500) {
        message = 'A server error occurred. Please try again later.';
      }

      snackBar.open(message, 'Dismiss', {
        duration: 5000,
        panelClass: ['snack-error'],
        horizontalPosition: 'right',
        verticalPosition: 'top',
      });

      return throwError(() => error);
    })
  );
};
