import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';
import { TokenService } from '../../services/token.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  email = '';
  password = '';
  showPassword = false;

  constructor(
    private authService: AuthService,
    private tok: TokenService,
    private router: Router
  ) {}

  // ============================
  // LOGIN HANDLER
  // ============================
  onLogin() {
    const payload = {
      email: this.email,
      password: this.password,
    };

     this.authService.login(payload).subscribe({
    next: (res) => {
      console.log('LOGIN SUCCESS', res);

      // 🔐 Store tokens
      this.tok.setTokens(res.accessToken, res.refreshToken);

      // ➡️ Redirect to dashboard
      this.router.navigate(['/dashboard']);
    },
    error: (err) => {
      console.error('LOGIN ERROR', err.error || err);
    },
  });
  }
}
