using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServerApp.Models;

namespace ServerApp.DTO
{
    public class MessageResultDto
    {
        public int SenderId { get; set; }
        public string Sender { get; set; }

        public int RecipientId { get; set; }
        public string Recipient { get; set; }
        public string Text { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateRead { get; set; }

        public bool IsRead { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }   
    }
}