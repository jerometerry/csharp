namespace CodeContracts
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTests();
        }

        static void RunTests()
        {
            var mst = new MyStackTests(-1, 5);
            mst.Test();
        }
    }
}
