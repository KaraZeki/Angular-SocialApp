using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class UserToUser
    {
        public int UserId { get; set; }
        public User User { get; set; }        
        public int FollowerId { get; set; }
        public User Follower { get; set; }
    }
}