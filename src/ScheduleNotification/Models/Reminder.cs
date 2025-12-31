using System;

namespace ScheduleNotification.Models
{
    public class Reminder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueTime { get; set; }
        public bool IsCompleted { get; set; }
        public RepeatType Repeat { get; set; } = RepeatType.None;
    }

    public enum RepeatType
    {
        None,
        Daily,
        Weekly,
        Monthly
    } 
}
