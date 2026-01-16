import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InstructorService } from '../../services/instructor.service';
import { ToastService } from '../../services/toast.service';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { environment } from '../../../environments/environment';
import { ChatService } from '../../services/chat.service';
import { AuthService } from '../../services/auth';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './instructor-dashboard.html',
})
export class InstructorDashboard implements OnInit {

  // ============================
  // STATE
  // ============================
  instructor: any;

  clients: any[] = [];
  bookings: any[] = [];

  loadingClients = true;
  loadingBookings = true;

  // Profile edit modal
  showEditProfile = false;
  selectedImageFile?: File;

  // Editable fields
  editProfile = {
    fullName: '',
    specialty: '',
    bio: ''
  };

  // Image helpers
  apiUrl = environment.apiUrl;
  cacheBuster = Date.now();

  // Unread messages
  unreadCounts: { [key: string]: number } = {};
  currentUserId = '';

  constructor(
    private instructorService: InstructorService,
    private toast: ToastService,
    private chatService: ChatService,
    private authService: AuthService
  ) {}

  // ============================
  // INIT
  // ============================
  ngOnInit(): void {
    this.currentUserId = this.authService.getUserId() || '';
    this.loadInstructorProfile();
    this.loadClients();
    this.loadBookings();
    this.loadUnreadCounts();
    
    // Poll for new messages every 5 seconds
    setInterval(() => this.loadUnreadCounts(), 5000);
  }

  // ============================
  // LOAD PROFILE
  // ============================
  loadInstructorProfile() {
    this.instructorService.getMyProfile().subscribe({
      next: res => {
        this.instructor = res;
        this.cacheBuster = Date.now();
      },
      error: () => this.toast.error('Failed to load profile')
    });
  }

  // ============================
  // LOAD CLIENTS
  // ============================
  loadClients() {
    this.loadingClients = true;
    this.instructorService.getMyClients().subscribe({
      next: res => {
        this.clients = res;
        this.loadingClients = false;
      },
      error: () => {
        this.loadingClients = false;
        this.toast.error('Failed to load clients');
      }
    });
  }

  // ============================
  // LOAD BOOKINGS
  // ============================
  loadBookings() {
    this.loadingBookings = true;
    this.instructorService.getBookings().subscribe({
      next: res => {
        this.bookings = res;
        this.loadingBookings = false;
      },
      error: () => {
        this.loadingBookings = false;
        this.toast.error('Failed to load bookings');
      }
    });
  }

  // ============================
  // LOAD UNREAD MESSAGE COUNTS
  // ============================
  loadUnreadCounts(): void {
    if (!this.currentUserId) return;

    this.chatService.getUnreadCountsByUser(this.currentUserId).subscribe({
      next: (counts) => {
        this.unreadCounts = counts;
      },
      error: (err) => {
        console.error('Failed to load unread counts', err);
      }
    });
  }

  // ============================
  // GET UNREAD COUNT FOR CLIENT
  // ============================
  getUnreadCount(clientId: string): number {
    return this.unreadCounts[clientId] || 0;
  }

  // ============================
  // CANCEL BOOKING
  // ============================
  cancelBooking(id: number) {
    if (!confirm('Are you sure you want to cancel this booking?')) return;

    this.instructorService.cancelBooking(id).subscribe({
      next: () => {
        this.bookings = this.bookings.filter(b => b.id !== id);
        this.toast.success('Booking cancelled');
      },
      error: () => this.toast.error('Failed to cancel booking')
    });
  }

  // ============================
  // EDIT PROFILE
  // ============================
  openEditProfile() {
    this.editProfile = {
      fullName: this.instructor?.user?.fullName ?? '',
      specialty: this.instructor?.specialty ?? '',
      bio: this.instructor?.bio ?? ''
    };
    this.showEditProfile = true;
  }

  closeEditProfile() {
    this.showEditProfile = false;
    this.selectedImageFile = undefined;
  }

  onImageSelected(event: any) {
    this.selectedImageFile = event.target.files?.[0];
  }

  saveProfile() {
    // 1️⃣ Update text fields
    this.instructorService.updateProfile(this.editProfile).subscribe({
      next: () => {
        this.instructor.user.fullName = this.editProfile.fullName;
        this.instructor.specialty = this.editProfile.specialty;
        this.instructor.bio = this.editProfile.bio;
      },
      error: () => this.toast.error('Failed to update profile')
    });

    // 2️⃣ Upload image (optional)
    if (this.selectedImageFile) {
      const formData = new FormData();
      formData.append('file', this.selectedImageFile);

      this.instructorService.uploadProfileImage(formData).subscribe({
        next: res => {
          this.instructor.profileImageUrl = res.profileImageUrl;
          this.cacheBuster = Date.now();
        },
        error: () => this.toast.error('Image upload failed')
      });
    }

    this.toast.success('Profile updated');
    this.closeEditProfile();
  }
}