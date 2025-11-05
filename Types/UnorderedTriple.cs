using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EinheitsKiste
{
    public class UnorderedTriple<T> : Tuple<T, T, T>
    {
        public UnorderedTriple(T item1, T item2, T item3) : base(item1, item2, item3) { }

        public HashSet<T> ToSet() => new() { Item1, Item2, Item3 };

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            var other = (UnorderedTriple<T>)obj;
            return ToSet().All(other.ToSet().Contains);
        }

        public override int GetHashCode()
        {
            List<int> list = new() {Item1.GetHashCode(), Item2.GetHashCode(), Item3.GetHashCode()};
            list.Sort();
            return (list[0], list[1], list[2]).GetHashCode();
        }
    }
}
