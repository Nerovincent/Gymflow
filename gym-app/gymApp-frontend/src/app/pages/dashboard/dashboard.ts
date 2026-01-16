import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TokenService } from '../../services/token.service';
import { Roles } from '../../models/roles';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `<p>Redirecting...</p>`,
})
export class Dashboard implements OnInit {

  constructor(
    private tokenService: TokenService,
    private router: Router
  ) {}

  ngOnInit(): void {
    console.log('DASHBOARD HIT');
    if (this.tokenService.hasRole(Roles.Admin)) {
      this.router.navigate(['/admin']);
      return;
    }

    if (this.tokenService.hasRole(Roles.Instructor)) {
      this.router.navigate(['/instructor']);
      return;
    }

    if (this.tokenService.hasRole(Roles.Client)) {
      this.router.navigate(['/client']);
      return;
    }

    // Fallback (should never happen if authGuard works)
    this.router.navigate(['/login']);
  }
}
