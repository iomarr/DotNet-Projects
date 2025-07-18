using System;
using System.ComponentModel.DataAnnotations;

namespace Freelancer_Task.Models
{
    public enum ProjectStatus
    {
        [Display(Name = "Not Started")]
        NotStarted = 0,

        [Display(Name = "Active")]
        Active = 1,

        [Display(Name = "On Hold")]
        OnHold = 2,

        [Display(Name = "Completed")]
        Completed = 3
    }

    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public DateTime? EndDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<PTask> Tasks { get; set; } = new List<PTask>();

    }
}