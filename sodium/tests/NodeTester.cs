namespace sodium.tests
{
    using NUnit.Framework;
    using NUnit;
    [TestFixture]
    public class NodeTester
    {
        [Test]
        public void TestNode()
        {
            Node a = new Node(0);
            Node b = new Node(1);
            a.linkTo(b);
            Assert.True(a.CompareTo(b) < 0);
        }
    }
}
