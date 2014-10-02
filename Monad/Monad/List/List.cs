using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Monad.List
{
    /**
     * A result value in the list monad, which consists of a list of all valid
     * results of the computation.
     *
     * @author Dave Herman
     */
    public class List : IResut
    {
        /** a cached empty list. */
        private static readonly ArrayList EmptyArrayList = new ArrayList();

        /** a cached empty result list. */
        internal static List Empty = new List();

        /**
         * Appends two lists of results.
         *
         * @param l1 the first list.
         * @param l2 the second list.
         * @return the appended list.
         */
        internal static List Append(List l1, List l2)
        {
            var result = new ArrayList(l1.list.Count + l2.list.Count);
            result.AddRange(l1.list);
            result.AddRange(l2.list);
            return new List(result);
        }

        /** the internal list of results. */
        private readonly System.Collections.ArrayList list;

        /**
         * Constructs a new list containing one single result.
         *
         * @param value the single result.
         */
        public List(Object value)
        {
            this.list = new ArrayList { value };
        }

        /**
         * Constructs a new list containing the given list of results.
         *
         * @param list the results.
         */
        private List(ArrayList list)
        {
            this.list = list;
        }

        /**
         * Constructs a new, empty list of results.
         */
        private List()
            : this(EmptyArrayList)
        {
        }

        /**
         * Maps the given function across the list of results and returns a list
         * consisting of the flattened contents of all the generated lists of
         * results.
         *
         * @param f the function to map across the list of results.
         */
        internal List MapAppend(IComputationFactory f)
        {
            var result = new ArrayList();
            foreach (var o in list)
            {
                var ri = (List)f.Make(o).Apply();
                result.AddRange(ri.list);
            }
            return new List(result);
        }

        /**
         * Returns a {@link java.util.List} of the results.
         *
         * @return a list of the results.
         */
        public Object Value()
        {
            return list;
        }
    }
}
