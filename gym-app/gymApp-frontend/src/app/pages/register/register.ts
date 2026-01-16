import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../services/auth';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
})
export class Register {

  fullName = '';
  email = '';
  password = '';
  confirmPassword = '';

  showPassword = false;
  showConfirmPassword = false;

  submitted = false;

  constructor(
    private authService: AuthService,
    private toast: ToastService,
    private router: Router
  ) {}

  // ============================
  // REGISTER HANDLER
  // ============================
  onRegister(form: NgForm) {
    this.submitted = true;

    // Stop if form is invalid
    if (form.invalid) {
      return;
    }

    // Password match check
    if (this.password !== this.confirmPassword) {
      this.toast.error('Passwords do not match');
      return;
    }

    const payload = {
      fullName: this.fullName,
      email: this.email,
      password: this.password,
    };

    this.authService.register(payload).subscribe({
      next: () => {
        this.toast.success('Registration successful');
        this.router.navigate(['/login']);
      },
      error: err => {
        this.toast.error(err?.error || 'Registration failed');
      }
    });
  }
}
