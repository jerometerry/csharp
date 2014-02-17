namespace sodium.tests
{
    using System;

    public class MemoryTest5
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

            EventSink<Int32> eChange = new EventSink<Int32>();
            Behavior<Int32> o = eChange.Hold(0);
            IListener l = o.Value().Listen(tt => {
                //System.out.println(tt)
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