using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRChat.Models
{
    public class ChatInfo
    {
        public User FromUser { get; set; }

        public User ToUser { get; set; }
    }
}
