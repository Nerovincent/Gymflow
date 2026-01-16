import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ClientBookingSlotsService {
  private baseUrl = 'http://localhost:5000/api/client/booking-slots';

  constructor(private http: HttpClient) {}

  getSlots(trainerUserId: string, date: string) {
    const params = new HttpParams()
      .set('trainerUserId', trainerUserId)
      .set('date', date);

    return this.http.get<any[]>(this.baseUrl, { params });
  }
}
