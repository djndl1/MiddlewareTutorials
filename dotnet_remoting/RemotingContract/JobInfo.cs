using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemotingContract;

[Serializable]
public struct JobInfo(int id, string description, string assignedUser, string status)
{
    public int Id { get; set; } = id;

    public string Description { get; set; } = description;

    public string AssignedUser { get; set; } = assignedUser;

    public string Status { get; set; } = status;
}
