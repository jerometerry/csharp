using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sodium
{
    public class Maybe<T>
    {
        private T _value;
        private bool _hasValue;

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
                return _value;
            }
        }
    }
}
