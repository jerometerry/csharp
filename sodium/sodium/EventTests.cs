using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace sodium
{
    [TestFixture]
    public class EventTests
    {
        [Test]
        public void TestMap()
        {
            EventSink<Int32> esb = new EventSink<Int32>();
            Event<string> map = esb.map<string>(a => a.ToString());
            Assert.IsNotNull(map);
            List<string> results = new List<string>();
            Listener listener = map.listen(results.Add);
            Assert.IsNotNull(listener);
            Assert.IsNotNull(map);
            esb.send(123);
            Assert.AreEqual("123", results[0]);
        }
    }
}
