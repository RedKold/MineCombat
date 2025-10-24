using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public interface ICloneable<T>
    {
        public T Clone();
    }

    public interface IMatchable<T>
    {
        bool Match(T t);
    }

    public interface IConstable<T> where T : IConstable<T>
    {
        public T ConstCast();
    }

    public delegate void Process<T>(ref T t);
}
