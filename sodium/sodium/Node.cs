namespace sodium {

    using System.Collections.Generic;
    //import java.util.HashSet;
    //import java.util.Set;

    public class Node : Comparable<Node> {
        public readonly static Node NULL = new Node(long.MaxValue);

	    public Node(long rank) {
		    this.rank = rank;
	    }

	    private long rank;
	    private Set<Node> listeners = new HashSet<Node>();

	    /**
	     * @return true if any changes were made. 
	     */
	    bool linkTo(Node target) {
		    if (target == NULL)
			    return false;

		    bool changed = target.ensureBiggerThan(rank, new HashSet<Node>());
		    listeners.Add(target);
		    return changed;
	    }

	    void unlinkTo(Node target) {
		    if (target == NULL)
			    return;

		    listeners.Remove(target);
	    }

	    private bool ensureBiggerThan(long limit, Set<Node> visited) {
		    if (rank > limit || visited.contains(this))
			    return false;

		    visited.Add(this);
		    rank = limit + 1;
		    foreach (Node l in listeners)
			    l.ensureBiggerThan(rank, visited);
		    return true;
	    }

	    public override int compareTo(Node o) {
		    if (rank < o.rank) return -1;
		    if (rank > o.rank) return 1;
		    return 0;
	    }
    }
}