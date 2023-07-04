using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemotingContract
{
    public struct JobInfo
    {
        public JobInfo(int id, string description, string assignedUser, string status)
        {
            Id = id;
            Description = description;
            AssignedUser = assignedUser;
            Status = status;
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public string AssignedUser { get; set; }

        public string Status { get; set; }
    }
}
