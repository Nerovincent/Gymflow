using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymFlow_Backend_Phase4E.Models
{
	public class InstructorAvailability
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int TrainerId { get; set; }

		[ForeignKey(nameof(TrainerId))]
		public Trainer Trainer { get; set; } = null!;

		[Required]
		public DayOfWeek DayOfWeek { get; set; }

		[Required]
		public TimeSpan StartTime { get; set; }

		[Required]
		public TimeSpan EndTime { get; set; }
	}
}
