namespace sodium {

	using System;
	using System.Collections.Generic;
    using System.Linq;

	public class EventLoop<A> : Event<A> {
		private Event<A> ea_out;

		public EventLoop()
		{
		}

		// TO DO: Copy & paste from EventSink. Can we improve this?
		private void send(Transaction trans, A a) {
			if (!firings.Any())
			    trans.last(new RunnableImpl(() => { firings.Clear(); }));
			firings.Add(a);

		    List<TransactionHandler<A>> listeners = new List<TransactionHandler<A>>(this.listeners);
    		foreach (TransactionHandler<A> action in listeners) {
    			try {
					action.run(trans, a);
    			}
    			catch (Exception ex)
    			{
    			    System.Diagnostics.Debug.WriteLine("{0}", ex);
    			}
    		}
		}

		public void loop(Event<A> ea_out)
		{
			if (this.ea_out != null)
				throw new ApplicationException("EventLoop looped more than once");
			this.ea_out = ea_out;
			EventLoop<A> me = this;
		    addCleanup(ea_out.listen_(this.node, new TransactionHandlerImpl<A>(me.send)));
		}
	}

}