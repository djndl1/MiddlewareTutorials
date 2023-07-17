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
    }
}
