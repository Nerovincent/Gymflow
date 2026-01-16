import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';

export const authGuard: CanActivateFn = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  const accessToken = tokenService.getAccessToken();
  const refreshToken = tokenService.getRefreshToken();

  // If BOTH tokens are missing logout
  if (!accessToken && !refreshToken) {
    router.navigate(['/login']);
    return false;
  }

  // Allow navigation
  return true;
};
