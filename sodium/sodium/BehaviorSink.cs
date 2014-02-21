namespace sodium
{
	public class BehaviorSink<A> : Behavior<A> {
		public BehaviorSink(A initValue) : base(new EventSink<A>(), initValue) {
		}
    
		public void send(A a)
		{
			((EventSink<A>)_event).send(a);
		}
	}
}