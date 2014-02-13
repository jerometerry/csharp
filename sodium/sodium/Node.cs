namespace sodium {

    //import java.util.HashSet;
    //import java.util.Set;

    public class Node : Comparable<Node> {
        public final static Node NULL = new Node(Long.MAX_VALUE);

	    Node(long rank) {
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
		    listeners.add(target);
		    return changed;
	    }

	    void unlinkTo(Node target) {
		    if (target == NULL)
			    return;

		    listeners.remove(target);
	    }

	    private bool ensureBiggerThan(long limit, Set<Node> visited) {
		    if (rank > limit || visited.contains(this))
			    return false;

		    visited.add(this);
		    rank = limit + 1;
		    for (Node l : listeners)
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