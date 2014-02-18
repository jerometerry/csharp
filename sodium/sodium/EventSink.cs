namespace sodium
{

    public class EventSink<TEvent> : Event<TEvent>
    {
        public void Send(TEvent evt)
        {
            var sink = this;
            Transaction.Run(t => sink.Send(t, evt));
        }
    }
}