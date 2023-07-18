using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RemotingContract;

namespace RemotingJobServer
{
    public class JobNotesInternal : MarshalByRefObject, IJobNotes
    {
        public void AddNote(int id, string s)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNotes(int id)
        {
            throw new NotImplementedException();
        }
    }
}
