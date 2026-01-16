import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from './../../../services/toast.service';
import { InstructorAvailabilityService } from './../../../services/instructor-availability.service';
import { RouterLink } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-instructor-availability',
  templateUrl: './instructor-availability.html',
  imports: [CommonModule,RouterLink]
})
export class InstructorAvailabilityComponent implements OnInit {
    isSubmitting = false;
  days = [
    { label: 'Mon', value: 1 },
    { label: 'Tue', value: 2 },
    { label: 'Wed', value: 3 },
    { label: 'Thu', value: 4 },
    { label: 'Fri', value: 5 },
    { label: 'Sat', value: 6 },
    { label: 'Sun', value: 0 }
  ];

  availability: any[] = [];

  constructor(
    private availabilityService: InstructorAvailabilityService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadAvailability();
  }

  loadAvailability(): void {
    this.availabilityService.getAvailability()
      .subscribe({
        next: data => this.availability = data,
        error: err => console.error('Failed to load availability', err)
      });
  }

  addSlot(day: number, start: string, end: string): void {
  if (!start || !end || this.isSubmitting) return;

  this.isSubmitting = true;

  const payload = {
    dayOfWeek: day,
    startTime: start.length === 5 ? `${start}:00` : start,
    endTime: end.length === 5 ? `${end}:00` : end
  };

  this.availabilityService.addAvailability(payload)
    .subscribe({
      next: () => {
        this.toast.success('Availability added');
        console.log('Added availability with payload:', payload);
        this.loadAvailability();
        this.isSubmitting = false;
      },
      error: err => {
        this.toast.error(
          err?.error ?? 'Invalid or overlapping availability slot'
        );
        this.isSubmitting = false;
      }
    });
}



  deleteSlot(id: number): void {
    if (!confirm('Delete this availability slot?')) return;

    this.availabilityService.deleteAvailability(id)
    .subscribe({
      next: () => {
        this.toast.success('Availability slot deleted');
        this.loadAvailability();
      },
      error: () => {
        this.toast.error('Failed to delete availability');
      }
    });
  }

  getSlotsForDay(day: number): any[] {
    return this.availability.filter(a => a.dayOfWeek === day);
  }
}
