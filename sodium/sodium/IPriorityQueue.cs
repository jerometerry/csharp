namespace sodium
{
    public interface IPriorityQueue<T>
    {
        void Add(T t);
        void Clear();
        bool Remove(T t);
        T Remove();
        T Peek();
        bool IsEmpty();
    }
}