using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelancer_Task.Models
{
    public class WorkLog
    {
        public DateTime Date { get; set; }
        public string ClientName { get; set; }
        public string ProjectName { get; set; }
        public string TaskTitle { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public PTaskStatus Status { get; set; }
    }
}
