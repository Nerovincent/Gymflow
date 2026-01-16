import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';

import { ChatService } from '../../../services/chat.service';
import { AuthService } from '../../../services/auth';
import { ToastService } from '../../../services/toast.service';

@Component({
  standalone: true,
  selector: 'app-instructor-chat',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './instructor-chat.html'
})
export class InstructorChatComponent implements OnInit {
  messages: any[] = [];
  newMessage = '';
  clientId = '';
  currentUserId = '';
  clientInfo: any = null;
  loading = true;
  sending = false;
  selectedImage: File | null = null;
  imagePreview: string | null = null;
  apiUrl = 'http://localhost:5000';

  constructor(
    private route: ActivatedRoute,
    private chatService: ChatService,
    private authService: AuthService,
    private toast: ToastService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.clientId = this.route.snapshot.paramMap.get('clientId') || '';
    this.currentUserId = this.authService.getUserId() || '';

    console.log('Current Instructor ID:', this.currentUserId);
    console.log('Client ID:', this.clientId);

    if (!this.currentUserId) {
      this.toast.error('Unable to identify current user');
      return;
    }

    this.loadClientInfo();
    this.loadMessages();
    
    // Mark messages as read when opening chat
    this.chatService.markAsRead(this.currentUserId, this.clientId).subscribe();
    
    // Poll for new messages every 3 seconds
    setInterval(() => this.loadMessages(), 3000);
  }

  loadClientInfo(): void {
    this.http.get(`${this.apiUrl}/api/auth/user/${this.clientId}`).subscribe({
      next: (data: any) => {
        this.clientInfo = data;
        console.log('Client Info:', this.clientInfo);
      },
      error: (err) => {
        console.error('Failed to load client info', err);
        this.toast.error('Failed to load client information');
      }
    });
  }

  loadMessages(): void {
    this.chatService.getConversation(this.currentUserId, this.clientId).subscribe({
      next: (data) => {
        this.messages = data;
        this.loading = false;
        setTimeout(() => this.scrollToBottom(), 100);
      },
      error: (err) => {
        console.error('Failed to load messages', err);
        this.loading = false;
      }
    });
  }

  onImageSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedImage = file;

      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(): void {
    this.selectedImage = null;
    this.imagePreview = null;
  }

  sendMessage(): void {
    if (!this.newMessage.trim() && !this.selectedImage) {
      return;
    }

    if (!this.currentUserId) {
      this.toast.error('Unable to send message: User not identified');
      return;
    }

    this.sending = true;

    this.chatService.sendMessage(
      this.currentUserId,
      this.clientId,
      this.newMessage.trim() || undefined,
      this.selectedImage || undefined
    ).subscribe({
      next: (response) => {
        console.log('Message sent successfully:', response);
        this.newMessage = '';
        this.selectedImage = null;
        this.imagePreview = null;
        this.loadMessages();
        this.sending = false;
      },
      error: (err) => {
        console.error('Failed to send message:', err);
        this.toast.error('Failed to send message');
        this.sending = false;
      }
    });
  }

  scrollToBottom(): void {
    const chatContainer = document.getElementById('chat-messages');
    if (chatContainer) {
      chatContainer.scrollTop = chatContainer.scrollHeight;
    }
  }

  isMyMessage(message: any): boolean {
    return message.senderId === this.currentUserId;
  }
}