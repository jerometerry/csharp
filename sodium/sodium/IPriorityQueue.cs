namespace sodium
{
    public interface IPriorityQueue<T>
    {
        void Add(T t);
        void Clear();
        void Remove(T t);
        T Remove();
        bool IsEmpty();
    }
}