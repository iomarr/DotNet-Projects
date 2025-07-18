using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelancer_Task.Models
{
        public class Client
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Company { get; set; }
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime? ModifiedDate { get; set; }
        }

   
}
