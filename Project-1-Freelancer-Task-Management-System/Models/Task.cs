using System;
using System.ComponentModel.DataAnnotations;

namespace Freelancer_Task.Models
{
    public enum PTaskStatus
    {
        [Display(Name = "To Do")]
        ToDo = 0,

        [Display(Name = "In Progress")]
        InProgress = 1,

        [Display(Name = "Done")]
        Done = 2,

        [Display(Name = "Blocked")]
        Blocked = 3
    }
    public class PTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; } 
        public PTaskStatus Status { get; set; } = PTaskStatus.ToDo;

        // Time Tracking
        public DateTime? TimerStart { get; set; } = DateTime.Now;
        public TimeSpan? TotalTimeSpent { get; set; }

        // Relationships
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}