using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RemotingContract;

namespace RemotingJobServer
{
    public class JobNotesFactory : MarshalByRefObject, IJobNotesFactory
    {
        public IJobNotes Create()
        {
            return new JobNotesInternal();
        }
    }
}
