import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { TokenService } from '../services/token.service';
import { catchError, switchMap, throwError } from 'rxjs';

let isRefreshing = false;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const http = inject(HttpClient);

  const accessToken = tokenService.getAccessToken();

  // Attach access token
  if (accessToken) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {

      // If NOT 401, just throw error
      if (error.status !== 401) {
        return throwError(() => error);
      }

      // Prevent infinite refresh loops
      if (isRefreshing) {
        return throwError(() => error);
      }

      isRefreshing = true;

      const refreshToken = tokenService.getRefreshToken();

      if (!refreshToken) {
        tokenService.clearTokens();
        location.href = '/login';
        return throwError(() => error);
      }

      // Call refresh endpoint
      return http.post<any>(
        'http://localhost:5000/api/auth/refresh',
        { refreshToken }
      ).pipe(
        switchMap((res) => {
          isRefreshing = false;

          // Save new tokens
          tokenService.setTokens(res.accessToken, res.refreshToken);

          // Retry original request
          const retryReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${res.accessToken}`,
            },
          });

          return next(retryReq);
        }),
        catchError((refreshErr) => {
          isRefreshing = false;
          tokenService.clearTokens();
          location.href = '/login';
          return throwError(() => refreshErr);
        })
      );
    })
  );
};
