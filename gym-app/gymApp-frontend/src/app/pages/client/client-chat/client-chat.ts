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
  selector: 'app-client-chat',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './client-chat.html'
})
export class ClientChatComponent implements OnInit {
  messages: any[] = [];
  newMessage = '';
  trainerId = '';
  currentUserId = '';
  trainerInfo: any = null;
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
    // Get IDs first
    this.trainerId = this.route.snapshot.paramMap.get('trainerId') || '';
    this.currentUserId = this.authService.getUserId() || '';

    // Debug: Check if user ID is retrieved
    console.log('Current User ID:', this.currentUserId);
    console.log('Trainer ID:', this.trainerId);

    if (!this.currentUserId) {
      this.toast.error('Unable to identify current user');
      return;
    }

    this.loadTrainerInfo();
    this.loadMessages();
    
    // Mark messages as read AFTER getting the IDs
    this.chatService.markAsRead(this.currentUserId, this.trainerId).subscribe({
      next: () => console.log('Messages marked as read'),
      error: (err) => console.error('Failed to mark messages as read', err)
    });
    
    // Poll for new messages every 3 seconds
    setInterval(() => this.loadMessages(), 3000);
  }

  loadTrainerInfo(): void {
    this.http.get(`${this.apiUrl}/api/trainers/user/${this.trainerId}`).subscribe({
      next: (data: any) => {
        this.trainerInfo = data;
        console.log('Trainer Info:', this.trainerInfo);
      },
      error: (err) => {
        console.error('Failed to load trainer info', err);
        this.toast.error('Failed to load trainer information');
      }
    });
  }

  loadMessages(): void {
    this.chatService.getConversation(this.currentUserId, this.trainerId).subscribe({
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

      // Create image preview
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

    console.log('Sending message:', {
      senderId: this.currentUserId,
      receiverId: this.trainerId,
      message: this.newMessage,
      hasImage: !!this.selectedImage
    });

    this.chatService.sendMessage(
      this.currentUserId,
      this.trainerId,
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
        console.error('Error details:', err.error);
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