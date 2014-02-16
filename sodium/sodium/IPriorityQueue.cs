namespace sodium
{
    public interface IPriorityQueue<T>
    {
        void Add(T item);
        void Clear();
        bool Remove(T item);
        T Remove();
        T Peek();
        bool IsEmpty();
    }
}