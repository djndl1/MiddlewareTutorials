using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RemotingContract;
using System.Runtime.Remoting.Lifetime;

namespace RemotingJobServer
{
    public class JobNotesInternal : MarshalByRefObject, IJobNotes
    {
        private readonly Dictionary<int, List<string>> _notes;

        public JobNotesInternal()
        {
            _notes = new Dictionary<int, List<string>>();
        }

        public void AddNote(int id, string s)
        {
            if (!_notes.TryGetValue(id, out List<string> notes))
            {
                notes = new List<string>();
                _notes.Add(id, notes);
            }
            notes.Add(DateTime.Now.ToString("O") + Environment.NewLine + s);
        }

        public List<string> GetNotes(int id)
        {
            return !_notes.TryGetValue(id, out List<string> notes)
                ? new List<string>() : notes;
        }

        public override object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease != null)
            {
                lease.InitialLeaseTime = TimeSpan.FromMinutes(1);
                lease.SponsorshipTimeout = lease.InitialLeaseTime;
                lease.RenewOnCallTime = TimeSpan.FromMinutes(0.5);
            }
            return lease;
        }
    }
}
