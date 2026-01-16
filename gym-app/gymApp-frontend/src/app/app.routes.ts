import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Dashboard } from './pages/dashboard/dashboard';

import { authGuard } from './guards/auth-guard';
import { roleGuard } from './guards/role-guard';
import { Roles } from './models/roles';

export const routes: Routes = [
  // ================= PUBLIC =================
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  // ================= COMMON =================
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard]
  },

  // ================= ADMIN =================
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard([Roles.Admin])],
    loadComponent: () =>
      import('./pages/admin/admin-dashboard')
        .then(m => m.AdminDashboard)
  },

  // ================= INSTRUCTOR =================
  {
    path: 'instructor',
    canActivate: [authGuard, roleGuard([Roles.Instructor])],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/instructor/instructor-dashboard')
            .then(m => m.InstructorDashboard)
      },
      {
        path: 'availability',
        loadComponent: () =>
          import('../app/pages/instructor/availability/instructor-availability')
            .then(m => m.InstructorAvailabilityComponent)
      },
      {
        path: 'chat/:clientId',
        loadComponent: () =>
          import('./pages/instructor/instructor-chat/instructor-chat')
            .then(m => m.InstructorChatComponent)
      }
    ]
  },

  // ================= CLIENT =================
  {
    path: 'client',
    canActivate: [authGuard, roleGuard([Roles.Client])],
    loadComponent: () =>
      import('./pages/client/client-dashboard')
        .then(m => m.ClientDashboard)
  },
  {
    path: 'client/book/:trainerUserId', 
    canActivate: [authGuard, roleGuard([Roles.Client])],
    loadComponent: () =>
      import('./pages/client/client-booking/client-booking')
        .then(m => m.ClientBookingComponent)
  },
  {
    path: 'client/chat/:trainerId', 
    canActivate: [authGuard, roleGuard([Roles.Client])],
    loadComponent: () =>
      import('./pages/client/client-chat/client-chat')
        .then(m => m.ClientChatComponent)
  },

  // ================= UNAUTHORIZED =================
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./pages/unauthorized/unauthorized')
        .then(m => m.Unauthorized)
  },

  // ================= FALLBACK =================
  { path: '**', redirectTo: 'login' }
];