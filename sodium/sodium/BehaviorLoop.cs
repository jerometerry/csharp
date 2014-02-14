namespace sodium
{
    public sealed class BehaviorLoop<TA> : Behavior<TA>
    {
        public BehaviorLoop()
            : base(new EventLoop<TA>(), default(TA))
        {
        }

        public void Loop(Behavior<TA> aOut)
        {
            ((EventLoop<TA>)Evt).Loop(aOut.Updates());
            Value = aOut.Sample();
        }
    }
}