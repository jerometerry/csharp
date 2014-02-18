namespace sodium
{
    using System;

    public class Maybe<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        public static Maybe<T> Null
        {
            get
            {
                return new Maybe<T>(default(T), false);
            }
        }

        public Maybe()
        {
            _hasValue = false;
        }

        public Maybe(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public Maybe(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        public bool HasValue
        {
            get
            {
                return _hasValue;
            }
        }

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new ArgumentException("Maybe doesn't contain a value!");
                }
                return _value;
            }
        }

        public static implicit operator T(Maybe<T> m)
        {
            return m.Value;
        }

        public static implicit operator Maybe<T>(T m)
        {
            return new Maybe<T>(m);
        }

        public override string ToString()
        {
            if (HasValue)
            {
                return _value.ToString();
            }
            return string.Empty;
        }

        public override bool Equals(object obj)
        {
            var m = obj as Maybe<T>;
            if (m == null)
            {
                return false;
            }

            if (HasValue != m.HasValue)
                return false;

            return Value.Equals(m.Value);
        }

        public override int GetHashCode()
        {
            return !HasValue ? 0 : Value.GetHashCode();
        }
    }
}
