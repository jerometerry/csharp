namespace sodium.tests
{
    using System;

    public class MemoryTest4
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

            EventSink<Int32> et = new EventSink<Int32>();
            EventSink<Int32> eChange = new EventSink<Int32>();
            Behavior<Event<Int32>> oout = eChange.Map(x => (Event<Int32>)et).Hold((Event<Int32>)et);
            Event<Int32> o = Behavior<Int32>.SwitchE(oout);
            IListener l = o.Listen(tt => {
                Console.WriteLine("{0}", tt);
            });
            int i = 0;
            while (i < 1000000000) {
                eChange.Send(i);
                i++;
            }
            l.Unlisten();
        }
    }
}