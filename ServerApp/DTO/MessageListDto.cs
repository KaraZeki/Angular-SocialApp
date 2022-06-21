using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerApp.Models;

namespace ServerApp.DTO
{
    public class MessageListDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public string LastMessage { get; set; }
        
    }
}