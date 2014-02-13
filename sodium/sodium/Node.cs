namespace sodium {

    using System;
    using System.Collections.Generic;

    public class Node : IComparable<Node> {
        public readonly static Node NULL = new Node(long.MaxValue);

	    public Node(long rank) {
		    this.rank = rank;
	    }

	    private long rank;
	    private ISet<Node> listeners = new HashSet<Node>();

	    /**
	     * @return true if any changes were made. 
	     */
	    public bool linkTo(Node target) {
		    if (target == NULL)
			    return false;

		    bool changed = target.ensureBiggerThan(rank, new HashSet<Node>());
		    listeners.Add(target);
		    return changed;
	    }

	    public void unlinkTo(Node target) {
		    if (target == NULL)
			    return;

		    listeners.Remove(target);
	    }

	    private bool ensureBiggerThan(long limit, ISet<Node> visited) {
		    if (rank > limit || visited.Contains(this))
			    return false;

		    visited.Add(this);
		    rank = limit + 1;
		    foreach (Node l in listeners)
			    l.ensureBiggerThan(rank, visited);
		    return true;
	    }

	    public int CompareTo(Node o) {
		    if (rank < o.rank) return -1;
		    if (rank > o.rank) return 1;
		    return 0;
	    }
    }
}