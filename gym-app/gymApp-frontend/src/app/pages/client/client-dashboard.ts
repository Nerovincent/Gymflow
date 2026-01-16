import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { ChatService } from '../../services/chat.service';
import { AuthService } from '../../services/auth';

@Component({
  standalone: true,
  selector: 'app-client-dashboard',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './client-dashboard.html',
})
export class ClientDashboard implements OnInit {

  trainers: any[] = [];
  loading = true;
  apiUrl = environment.apiUrl;
  searchSpecialty = '';
  unreadCounts: { [key: string]: number } = {};
  currentUserId = '';

  constructor(
    private http: HttpClient,
    private chatService: ChatService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.getUserId() || '';
    this.loadTrainers();
    this.loadUnreadCounts();
    
    // Poll for new messages every 5 seconds
    setInterval(() => this.loadUnreadCounts(), 5000);
  }

  loadTrainers(): void {
    this.loading = true;
    
    this.http
      .get<any[]>('http://localhost:5000/api/trainers')
      .subscribe({
        next: data => {
          this.trainers = data;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
  }

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

  searchTrainers(): void {
    if (!this.searchSpecialty.trim()) {
      this.loadTrainers();
      return;
    }

    this.loading = true;

    this.http
      .get<any[]>('http://localhost:5000/api/trainers/search', {
        params: { specialty: this.searchSpecialty }
      })
      .subscribe({
        next: data => {
          this.trainers = data;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
  }

  clearSearch(): void {
    this.searchSpecialty = '';
    this.loadTrainers();
  }

  getUnreadCount(trainerId: string): number {
    return this.unreadCounts[trainerId] || 0;
  }
}