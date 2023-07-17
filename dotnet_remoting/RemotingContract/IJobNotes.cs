using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemotingContract
{
    public interface IJobNotesFactory
    {
        IJobNotes Create();
    }

    public interface IJobNotes
    {
        void AddNote(int id, string s);

        List<string> GetNotes(int id);
    }
}
