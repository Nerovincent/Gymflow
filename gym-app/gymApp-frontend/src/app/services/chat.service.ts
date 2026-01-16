import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private baseUrl = 'http://localhost:5000/api/chat';

  constructor(private http: HttpClient) {}

  sendMessage(senderId: string, receiverId: string, message?: string, image?: File): Observable<any> {
    const formData = new FormData();
    formData.append('senderId', senderId);
    formData.append('receiverId', receiverId);
    
    if (message) {
      formData.append('message', message);
    }
    
    if (image) {
      formData.append('image', image);
    }

    return this.http.post(`${this.baseUrl}/send`, formData);
  }

  getConversation(user1: string, user2: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/conversation`, {
      params: { user1, user2 }
    });
  }

  getUnreadCount(userId: string): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.baseUrl}/unread-count/${userId}`);
  }

  getUnreadCountsByUser(userId: string): Observable<{ [key: string]: number }> {
    return this.http.get<{ [key: string]: number }>(`${this.baseUrl}/unread-by-user/${userId}`);
  }

  markAsRead(userId: string, otherUserId: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/mark-read`, { userId, otherUserId });
  }
}