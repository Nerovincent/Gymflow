import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { ClientBookingSlotsService } from '../../../services/client-booking-slots.service';
import { BookingService } from '../../../services/booking.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  standalone: true,
  selector: 'app-client-booking',
  templateUrl: './client-booking.html',
  imports: [CommonModule, FormsModule, RouterLink]
})
export class ClientBookingComponent implements OnInit {

  trainerUserId!: string;

  // Selected date (any date in the week)
  selectedDate: string = '';

  // Weekly slots structure
  weekSlots: {
    label: string;
    date: string;
    slots: any[];
  }[] = [];

  constructor(
    private route: ActivatedRoute,
    private slotsService: ClientBookingSlotsService,
    private bookingService: BookingService,
    private toast: ToastService
  ) {
    const today = new Date();
    this.selectedDate = today.toISOString().split('T')[0];
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('trainerUserId');

    if (!id) {
      this.toast.error('Trainer not found');
      return;
    }

    this.trainerUserId = id;
    this.loadWeekSlots();
  }

  // ============================
  // BOOKING CONFIRMATION MODAL
  // ============================
  showConfirmModal = false;
  selectedSlot: any;

  openConfirmModal(slot: any) {
    this.selectedSlot = slot;
    this.showConfirmModal = true;
  }

  closeConfirmModal() {
    this.showConfirmModal = false;
    this.selectedSlot = null;
  }

  // UPDATED: Combined booking logic into confirmBooking
  confirmBooking() {
    const payload = {
      trainerUserId: this.trainerUserId,
      date: this.selectedSlot.date ?? this.selectedDate,
      startTime: this.selectedSlot.startTime,
      endTime: this.selectedSlot.endTime
    };

    this.bookingService.createBooking(payload).subscribe({
      next: () => {
        this.toast.success('Session booked');
        this.closeConfirmModal(); // UPDATED: Close modal first
        this.loadWeekSlots(); // UPDATED: Then refresh calendar
      },
      error: err => {
        // UPDATED: Better error message handling
        const errorMessage = err?.error?.message || err?.error || 'Slot already booked';
        this.toast.error(errorMessage);
        this.closeConfirmModal(); // UPDATED: Close modal even on error
        this.loadWeekSlots(); // UPDATED: Refresh in case slot was actually booked
      }
    });
  }

  // ============================
  // LOAD WEEKLY SLOTS
  // ============================
  loadWeekSlots(): void {
    this.weekSlots = [];

    const baseDate = new Date(this.selectedDate);
    const dayOfWeek = baseDate.getDay(); // 0 = Sunday
    const mondayOffset = dayOfWeek === 0 ? -6 : 1 - dayOfWeek;

    for (let i = 0; i < 7; i++) {
      const date = new Date(baseDate);
      date.setDate(baseDate.getDate() + mondayOffset + i);

      const dateStr = date.toISOString().split('T')[0];
      const label = date.toLocaleDateString('en-US', { weekday: 'short' });

      const dayBlock = {
        label,
        date: dateStr,
        slots: [] as any[]
      };

      this.weekSlots.push(dayBlock);

      // Load slots for this day
      this.slotsService
        .getSlots(this.trainerUserId, dateStr)
        .subscribe({
          next: slots => {
            dayBlock.slots = slots;
            console.log('Slots received:', slots);
          },
          error: () => {
            // silently fail per day
            dayBlock.slots = [];
          }
        });
    }
  }

  // REMOVED: bookSlot() method - logic moved to confirmBooking()
}