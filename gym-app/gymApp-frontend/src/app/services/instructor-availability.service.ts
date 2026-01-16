import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AddAvailabilityRequest } from '../models/AddAvailabilityRequest';

@Injectable({ providedIn: 'root' })
export class InstructorAvailabilityService {
  private baseUrl = 'http://localhost:5000/api/instructor/availability';

  constructor(private http: HttpClient) {}

  getAvailability(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl);
  }

  addAvailability(payload: AddAvailabilityRequest): Observable<any> {
    return this.http.post(this.baseUrl, payload);
  }

  deleteAvailability(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
