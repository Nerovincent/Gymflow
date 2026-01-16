import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InstructorService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // ============================
  // INSTRUCTOR → CLIENTS
  // ============================
  getMyClients() {
    return this.http.get<any[]>(
      `${this.apiUrl}/api/instructor/clients`
    );
  }

  // ============================
  // INSTRUCTOR → BOOKINGS
  // ============================
  getBookings() {
    return this.http.get<any[]>(
      `${this.apiUrl}/api/instructor/bookings`
    );
  }

  // ============================
  // INSTRUCTOR → CANCEL BOOKING
  // ============================
  cancelBooking(id: number) {
    return this.http.delete(
      `${this.apiUrl}/api/instructor/bookings/${id}`,
      { responseType: 'text' } // backend returns plain text
    );
  }

  // ============================
  // INSTRUCTOR → PROFILE
  // ============================
  getMyProfile() {
    return this.http.get<any>(
      `${this.apiUrl}/api/instructor/profile`
    );
  }

  // ============================
  // INSTRUCTOR → UPLOAD PROFILE IMAGE
  // ============================
  uploadProfileImage(formData: FormData) {
    return this.http.post<any>(
      `${this.apiUrl}/api/instructor/profile/image`,
      formData
    );
  }
  updateProfile(payload: any) {
  return this.http.put(`${this.apiUrl}/api/instructor/profile`, payload);
}

}
