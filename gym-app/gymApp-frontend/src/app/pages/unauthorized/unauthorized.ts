import { Component } from '@angular/core';

@Component({
  standalone: true,
  template: `
    <div class="flex flex-col items-center justify-center h-64">
      <h2 class="text-2xl font-bold text-error">Access Denied</h2>
      <p class="mt-2">You do not have permission to view this page.</p>
    </div>
  `
})
export class Unauthorized {}
