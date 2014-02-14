namespace sodium
{
    using System;
    using System.Collections.Generic;

    public class Node : IComparable<Node>
    {
        public readonly static Node Null = new Node(long.MaxValue);
        private long _rank;
        private readonly ISet<Node> _listeners = new HashSet<Node>();

        public Node(long rank)
        {
            _rank = rank;
        }

        /**
         * @return true if any changes were made. 
         */
        public bool LinkTo(Node target)
        {
            if (target == Null)
                return false;

            bool changed = target.EnsureBiggerThan(_rank, new HashSet<Node>());
            _listeners.Add(target);
            return changed;
        }

        public void UnlinkTo(Node target)
        {
            if (target == Null)
                return;

            _listeners.Remove(target);
        }

        private bool EnsureBiggerThan(long limit, ISet<Node> visited)
        {
            if (_rank > limit || visited.Contains(this))
                return false;

            visited.Add(this);
            _rank = limit + 1;
            foreach (Node l in _listeners)
                l.EnsureBiggerThan(_rank, visited);
            return true;
        }

        public int CompareTo(Node o)
        {
            if (_rank < o._rank) return -1;
            if (_rank > o._rank) return 1;
            return 0;
        }
    }
}