namespace sodium {

    public sealed class BehaviorSink<A> : Behavior<A> {
        public BehaviorSink(A initValue) {
    	    super(new EventSink<A>(), initValue);
        }
    
        public void send(A a)
        {
            ((EventSink<A>)evt).send(a);
        }
    }
}