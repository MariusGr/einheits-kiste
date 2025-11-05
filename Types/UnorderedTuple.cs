using System;
using System.Diagnostics.CodeAnalysis;


namespace EinheitsKiste
{
    public class UnorderedTuple<T> : Tuple<T, T>
    {
        public UnorderedTuple(T item1, T item2) : base(item1, item2) { }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            var other = (Tuple<T, T>) obj;
            return base.Equals(other) || base.Equals(new Tuple<T, T>(other.Item2, other.Item1));
        }

        public override int GetHashCode()
            => (Math.Min(Item1.GetHashCode(), Item2.GetHashCode()), Math.Max(Item1.GetHashCode(), Item2.GetHashCode())).GetHashCode();
    }
}
