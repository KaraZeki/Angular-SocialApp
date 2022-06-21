using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class Image
    {
        public int Id  { get; set; }
        public string Name { get; set; }
        public string Decription{ get; set; }
        public DateTime DateAdded{ get; set; }
        public bool IsProfile{ get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}