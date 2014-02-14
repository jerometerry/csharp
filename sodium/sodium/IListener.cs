namespace sodium
{
    public interface IListener
    {
        void Unlisten();
        IListener Append(IListener two);
    }
}
