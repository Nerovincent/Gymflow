import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToastService } from './services/toast.service';
import { Router } from '@angular/router';

import { TokenService } from '../app/services/token.service';
import { Roles } from './models/roles';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'gymApp-frontend';

  // expose roles to template
  Roles = Roles;

  constructor(
    private tokenService: TokenService,
    private router: Router,
    public toastService: ToastService
  ) {}

  isLoggedIn(): boolean {
    return this.tokenService.isLoggedIn();
  }

  hasRole(role: string): boolean {
    return this.tokenService.hasRole(role);
  }

  logout() {
    this.tokenService.clearTokens();
    this.router.navigate(['/login']);
  }
}
