import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  // Save tokens
  setTokens(accessToken: string, refreshToken: string) {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  // Get access token
  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  // Get refresh token
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  // Remove all tokens (LOGOUT)
  clearTokens() {
    console.warn('TOKENS CLEARED');
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  // Check login state
  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }

  setAccessToken(token: string) {
  localStorage.setItem('access_token', token);
}
getRoles(): string[] {
  const token = this.getAccessToken();
  if (!token) return [];

  const payload = JSON.parse(atob(token.split('.')[1]));

  // Support all common role claim formats
  const roles =
    payload['role'] ||
    payload['roles'] ||
    payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

  if (!roles) return [];

  return Array.isArray(roles) ? roles : [roles];
}

hasRole(role: string): boolean {
  return this.getRoles().includes(role);
}

hasAnyRole(roles: string[]): boolean {
  return roles.some(r => this.hasRole(r));
}
}
