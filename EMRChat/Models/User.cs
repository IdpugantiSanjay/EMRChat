using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRChat.Models
{

    public enum ApplicationType
    {
        Ehr = 1,
        Portal
    }


    public class User
    {
        public int PracticeId { get; set; }

        public int UserId { get; set; }

        public ApplicationType ApplicationType { get; set; }

        public string Username { get; set; }

        public HashSet<string> ConnectionIds { get; set; } = new HashSet<string>();

        public string UserIdentifier => $"{PracticeId}_{UserId}_{(int)ApplicationType}";

        public bool IsUserValid()
        {
            return PracticeId != default && UserId != default && ApplicationType != default;
        }

        //public override string ToString() => $"{PracticeId}_{UserId}_{(int)ApplicationType}";
    }
}
