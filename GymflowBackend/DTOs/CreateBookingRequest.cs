namespace GymFlow_Backend_Phase4E.Models
{
	public class CreateBookingRequest
	{
		public string TrainerUserId { get; set; } = null!;

		public DateTime Date { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
	}
}
