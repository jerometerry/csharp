namespace sodium
{
    using System.Collections.Generic;
    using System.Linq;

    public class PriorityQueue<T> : IPriorityQueue<T>
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            lock(_items)
            {
                _items.Add(item);
                _items.Sort();
            }
        }

        public void Clear()
        {
            lock(_items)
            {
                _items.Clear();
            }
        }

        public bool Remove(T item)
        {
            lock(_items)
            {
                return _items.Remove(item);
            }
        }

        public T Remove()
        {
            lock(_items)
            {
                var last = Peek();
                // TODO - can't assume nullable
                if (last != null)
                    Remove(last);
                return last;
            }
        }

        public T Peek()
        {
            lock(_items)
            {
                return _items.Last();
            }
        }

        public bool IsEmpty()
        {
            lock(_items)
            {
                return !_items.Any();
            }
        }
    }
}
