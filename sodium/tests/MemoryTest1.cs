namespace sodium.tests
{
    using System;
    using System.Threading;

    public class MemoryTest1
    {
        public static void main(String[] args)
        {
            //new Thread() {
            //    public void run()
            //    {
            //        try {
            //            while (true) {
            //                System.out.println("memory "+Runtime.getRuntime().totalMemory());
            //                Thread.sleep(5000);
            //            }
            //        }
            //        catch (InterruptedException e) {
            //            System.out.println(e.toString());
            //        }
            //    }
            //}.start();

            EventSink<Int32?> et = new EventSink<Int32?>();
            Behavior<Int32?> t = et.Hold(0);
            Event<Int32?> etens = et.Map(x => x/10);
            Event<Int32?> changeTens = et.Snapshot(t, (neu, old) =>
                neu.Equals(old) ? null : neu).FilterNotNull();
            Behavior<Behavior<Tuple2<Int32?,Int32?>>> oout =
                changeTens.Map(tens => t.Map(tt => new Tuple2<Int32?,Int32?>(tens, tt)))
                .Hold(t.Map(tt => new Tuple2<Int32?,Int32?>(0, tt)));
            Behavior<Tuple2<Int32?, Int32?>> o = Behavior<Tuple2<Int32?, Int32?>>.SwitchB(oout);
            IListener l = o.Value().Listen(tu => {
                //System.out.println(tu.a+","+tu.b);
            });
            int i = 0;
            while (i < 1000000000) {
                et.Send(i);
                i++;
            }
            l.Unlisten();
        }
    }
}