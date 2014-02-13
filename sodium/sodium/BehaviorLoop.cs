namespace sodium {

    public sealed class BehaviorLoop<A> : Behavior<A> {
        public BehaviorLoop() : base(new EventLoop<A>(), default(A)){
        }

        public void loop(Behavior<A> a_out)
        {
            ((EventLoop<A>)evt).loop(a_out.updates());
            _value = a_out.sample();
        }
    }

}