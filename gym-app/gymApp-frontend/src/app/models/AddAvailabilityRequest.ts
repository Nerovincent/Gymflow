export interface AddAvailabilityRequest {
  dayOfWeek: number;
  startTime: string;  // ✅ Changed to string
  endTime: string;    // ✅ Changed to string
}