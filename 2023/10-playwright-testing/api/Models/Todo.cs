using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTesting.Api.Models
{
    public class TodoModel
    {
        public int Id { get; set; } = 0;

        public string Description { get; set; }

        public string Status { get; set; } = "incompleted";

        public DateTime AddDate { get; set; } = DateTime.Now;

        public DateTime UpdateDate { get; set; } = DateTime.Now;
    }
}
