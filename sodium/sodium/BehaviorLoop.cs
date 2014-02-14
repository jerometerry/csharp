namespace sodium
{
    public sealed class BehaviorLoop<A> : Behavior<A>
    {
        public BehaviorLoop()
            : base(new EventLoop<A>(), default(A))
        {
        }

        public void loop(Behavior<A> a_out)
        {
            ((EventLoop<A>)Evt).loop(a_out.Updates());
            Value = a_out.Sample();
        }
    }
}