namespace sodium
{
    using System;

    public class Entry : IComparable<Entry>
    {
        private readonly Node _rank;
        public readonly IHandler<Transaction> Action;
        private static long _nextSeq;
        private readonly long _seq;

        public Entry(Node rank, IHandler<Transaction> action)
        {
            _rank = rank;
            Action = action;
            _seq = _nextSeq++;
        }

        public int CompareTo(Entry o)
        {
            int answer = _rank.CompareTo(o._rank);
            if (answer == 0)
            {  // Same rank: preserve chronological sequence.
                if (_seq < o._seq) answer = -1;
                else
                    if (_seq > o._seq) answer = 1;
            }
            return answer;
        }

    }
}
