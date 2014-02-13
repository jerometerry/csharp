namespace sodium {
    
    using System;
    using System.Collections.Generic;

    public sealed class Transaction {
        // Coarse-grained lock that's held during the whole transaction.
        private static readonly Object transactionLock = new Object();
        // Fine-grained lock that protects listeners and nodes.
        public static readonly Object listenersLock = new Object();

        // True if we need to re-generate the priority queue.
        public bool toRegen = false;

	    private class Entry : IComparable<Entry> {
		    private Node rank;
		    public IHandler<Transaction> action;
		    private static long nextSeq;
		    private long seq;

		    public Entry(Node rank, IHandler<Transaction> action) {
			    this.rank = rank;
			    this.action = action;
			    this.seq = nextSeq++;
		    }

		    public int CompareTo(Entry o) {
			    int answer = rank.CompareTo(o.rank);
			    if (answer == 0) {  // Same rank: preserve chronological sequence.
				    if (seq < o.seq) answer = -1; else
				    if (seq > o.seq) answer = 1;
			    }
			    return answer;
		    }

	    }

	    private PriorityQueue<Entry> prioritizedQ = new PriorityQueue<Entry>();
	    private ISet<Entry> entries = new HashSet<Entry>();
	    private List<IRunnable> lastQ = new List<IRunnable>(); 
	    private List<IRunnable> postQ;

	    public Transaction() {
	    }

	    private static Transaction currentTransaction;

	    /**
	     * Run the specified code inside a single transaction.
	     *
	     * In most cases this is not needed, because all APIs will create their own
	     * transaction automatically. It is useful where you want to run multiple
	     * reactive operations atomically.
	     */
	    public static void run(IRunnable code) {
            lock (transactionLock) {
                // If we are already inside a transaction (which must be on the same
                // thread otherwise we wouldn't have acquired transactionLock), then
                // keep using that same transaction.
                Transaction transWas = currentTransaction;
                try {
                    if (currentTransaction == null)
                        currentTransaction = new Transaction();
                    code.run();
                } finally {
                    if (transWas == null)
                        currentTransaction.close();
                    currentTransaction = transWas;
                }
            }
	    }

	    public static void run(IHandler<Transaction> code) {
            lock (transactionLock) {
                // If we are already inside a transaction (which must be on the same
                // thread otherwise we wouldn't have acquired transactionLock), then
                // keep using that same transaction.
                Transaction transWas = currentTransaction;
                try {
                    if (currentTransaction == null)
                        currentTransaction = new Transaction();
                    code.run(currentTransaction);
                } finally {
                    if (transWas == null)
                        currentTransaction.close();
                    currentTransaction = transWas;
                }
            }
	    }

        public static A apply<A>(ILambda1<Transaction, A> code) {
            lock (transactionLock) {
                // If we are already inside a transaction (which must be on the same
                // thread otherwise we wouldn't have acquired transactionLock), then
                // keep using that same transaction.
                Transaction transWas = currentTransaction;
                try {
                    if (currentTransaction == null)
                        currentTransaction = new Transaction();
                    return code.apply(currentTransaction);
                } finally {
                    currentTransaction.close();
                    currentTransaction = transWas;
                }
            }
	    }

	    public void prioritized(Node rank, IHandler<Transaction> action) {
	        Entry e = new Entry(rank, action);
		    prioritizedQ.Add(e);
		    entries.Add(e);
	    }

	    /**
         * Add an action to run after all prioritized() actions.
         */
	    public void last(IRunnable action) {
	        lastQ.Add(action);
	    }

	    /**
         * Add an action to run after all last() actions.
         */
	    public void post(IRunnable action) {
	        if (postQ == null)
	            postQ = new List<IRunnable>();
	        postQ.Add(action);
	    }

	    /**
	     * If the priority queue has entries in it when we modify any of the nodes'
	     * ranks, then we need to re-generate it to make sure it's up-to-date.
	     */
	    private void checkRegen()
	    {
	        if (toRegen) {
	            toRegen = false;
	            prioritizedQ.Clear();
	            foreach (Entry e in entries)
	                prioritizedQ.Add(e);
	        }
	    }

	    public void close() {
	        while (true) {
	            checkRegen();
		        if (prioritizedQ.isEmpty()) break;
		        Entry e = prioritizedQ.Remove();
		        entries.Remove(e);
			    e.action.run(this);
		    }
		    foreach (IRunnable action in lastQ)
			    action.run();
		    lastQ.Clear();
		    if (postQ != null) {
                foreach (IRunnable action in postQ)
                    action.run();
                postQ.Clear();
		    }
	    }
    }
}