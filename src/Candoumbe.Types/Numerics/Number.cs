using System;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics
{
    /// <summary>
    /// Base class for creating numeric types that can only be positive
    /// </summary>
    /// <typeparam name="TNumber">The numeric type that the current type will restrict to only non negative values.</typeparam>
    public abstract record Number<TNumber> : IComparable<Number<TNumber>>, IEquatable<Number<TNumber>>
        where TNumber : IComparable<TNumber>
    {
        /// <summary>
        /// Gets the underlying value
        /// </summary>
        /// <remarks>The value is garantied to be &gt; or equal to <c>0</c></remarks>
        public TNumber Value
        {
            get;
#if NET7_0_OR_GREATER
            init;
#else
            private set;
#endif
        } = default;

        /// <summary>
        /// Builds a new <see cref="Number{TNumber}"/> that contains the default value of <typeparamref name="TNumber"/>.
        /// </summary>
        public Number() => Value = default;

        /// <summary>
        /// Builds a new <see cref="Number{TNumber}"/> initialized with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public Number(TNumber value) => Value = value;

        ///<inheritdoc/>
        public int CompareTo(Number<TNumber> other) => Value.CompareTo(other.Value);

        ///<inheritdoc/>
        public virtual bool Equals(Number<TNumber> other) => Equals(Value, other.Value);

        ///<inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        ///<inheritdoc/>
        public static bool operator <(Number<TNumber> left, TNumber right)
            => left.Value.CompareTo(right) < 0;

        ///<inheritdoc/>
        public static bool operator >(Number<TNumber> left, TNumber right)
            => left.Value.CompareTo(right) > 0;

        ///<inheritdoc/>
        public static bool operator >=(Number<TNumber> left, TNumber right)
            => left.Value.CompareTo(right) >= 0;

        ///<inheritdoc/>
        public static bool operator <=(Number<TNumber> left, TNumber right)
            => left.Value.CompareTo(right) <= 0;
    }
}