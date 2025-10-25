using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public class Context : Properties
    {
#nullable enable
        private readonly Stack<Card> _pre = new();

        public IReadOnlyList<Card> PreCards => _pre.ToList();
        public Card? LastCard => _pre.Count == 0 ? null : _pre.Peek();
        public Card? FirstCard => _pre.Count == 0 ? null : _pre.Last();

        internal void Push(Card card)
        {
            _pre.Push(card);
        }

        internal Card? Pop()
        {
            return _pre.Count == 0 ? null : _pre.Pop();
        }

        internal void Clear()
        {
            _pre.Clear();
        }
#nullable disable
    }
}
