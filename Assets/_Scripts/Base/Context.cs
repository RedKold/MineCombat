using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public class Context : Properties
    {
        protected Box<string> _branches = new();

        public void SetBranch(string branch)
        {
            _branches.UpdateContent(branch);
        }

        public void SetBranch(params string[] branches)
        {
            _branches.UpdateContent(branches);
        } 
    }
}
