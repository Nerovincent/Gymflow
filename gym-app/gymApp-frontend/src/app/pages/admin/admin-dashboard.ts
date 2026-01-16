import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminService } from '../../services/admin.service';

import { ToastService } from '../../services/toast.service';

@Component({
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-dashboard.html'
})
export class AdminDashboard implements OnInit {

  users: any[] = [];
  loading = true;

  constructor(
    private adminService: AdminService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;

    this.adminService.getUsers().subscribe({
      next: res => {
        this.users = res;
        this.loading = false;
      },
      error: () => {
        this.toast.error('Failed to load users');
        this.loading = false;
      }
    });
  }

  promote(user: any) {
    this.adminService.promoteToInstructor(user.id).subscribe({
      next: () => {
        // ✅ Optimistically update UI
        user.roles.push('Instructor');

        // ✅ Success feedback
        this.toast.success('User promoted to Instructor');
      },
      error: err => {
        // Ignore auth timing issues
        if (err.status === 401 || err.status === 403) return;
        console.error(err);
        this.toast.error('Promotion failed');
      }
    });
  }

  deleteUser(user: any) {
    if (!confirm(`Are you sure you want to delete ${user.email}?`)) {
      return;
    }

    this.adminService.deleteUser(user.id).subscribe({
      next: () => {
        // ✅ Remove user from UI
        this.users = this.users.filter(u => u.id !== user.id);

        // ✅ Success feedback
        this.toast.success('User deleted successfully');
      },
      error: err => {
        console.error(err);
        this.toast.error('Failed to delete user');
      }
    });
  }

  isClient(user: any): boolean {
    return user.roles.includes('Client') && !user.roles.includes('Instructor');
  }

  isAdmin(user: any): boolean {
    return user.roles.includes('Admin');
  }
}