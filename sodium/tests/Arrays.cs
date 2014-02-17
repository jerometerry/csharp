namespace sodium.tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;

    internal static class Arrays<TA>
    {

        public static List<TA> AsList(params TA[] items)
        {
            return new List<TA>(items);
        }

        public static bool AreArraysEqual(List<TA> l1, List<TA> l2)
        {
            if (l1.Count != l2.Count)
                return false;

            l1.Sort();
            l2.Sort();

            for (int i = 0; i < l1.Count; i++)
            {
                TA item1 = l1[i];
                TA item2 = l2[i];
                if (!item1.Equals(item2))
                    return false;
            }

            return true;
        }

        public static void AssertArraysEqual(List<TA> l1, List<TA> l2)
        {
            Assert.True(Arrays<TA>.AreArraysEqual(l1, l2));
        }
    }
}
