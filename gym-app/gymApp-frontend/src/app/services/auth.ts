import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) {}

  // ============================
  // REGISTER USER
  // ============================
  register(payload: {
    fullName: string;
    email: string;
    password: string;
  }): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/api/auth/register`,
      payload,
      { responseType: 'text' } // backend returns plain text
    );
  }

  // ============================
  // LOGIN USER
  // ============================
  login(payload: { email: string; password: string }) {
    return this.http.post<{
      accessToken: string;
      refreshToken: string;
    }>(
      `${this.apiUrl}/api/auth/login`,
      payload
    );
  }

  // ============================
  // GET CURRENT USER ID
  // ============================
  getUserId(): string | null {
    const token = this.tokenService.getAccessToken();
    if (!token) {
      console.error('No token found');
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      console.log('Full JWT Payload:', payload); // Debug: see all claims
      
      // Try different possible claim names
      const userId = payload.sub || 
                     payload.userId || 
                     payload.nameid || 
                     payload.id ||
                     payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
      
      console.log('Extracted User ID:', userId);
      return userId || null;
    } catch (error) {
      console.error('Failed to decode token', error);
      return null;
    }
  }

  // ============================
  // GET ACCESS TOKEN
  // ============================
  getToken(): string | null {
    return this.tokenService.getAccessToken();
  }

  // ============================
  // CHECK IF USER IS AUTHENTICATED
  // ============================
  isAuthenticated(): boolean {
    return this.tokenService.isLoggedIn();
  }

  // ============================
  // LOGOUT USER
  // ============================
  logout(): void {
    this.tokenService.clearTokens();
  }
}