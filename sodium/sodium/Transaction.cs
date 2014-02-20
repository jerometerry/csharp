namespace sodium
{
    using System;
    using System.Collections.Generic;

    public class Transaction {
        // Coarse-grained lock that's held during the whole transaction.
        static Object transactionLock = new Object();
        // Fine-grained lock that protects listeners and nodes.
        internal static Object listenersLock = new Object();

        // True if we need to re-generate the priority queue.
        internal bool toRegen = false;
        
	    private class Entry : IComparable<Entry> {
		    private Node rank;
		    internal Handler<Transaction> action;
		    private static long nextSeq;
		    private long seq;

		    public Entry(Node rank, Handler<Transaction> action) {
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

        private List<Runnable> lastQ = new List<Runnable>(); 
        private List<Runnable> postQ;

        Transaction() {
        }

        private static Transaction currentTransaction;

        ///
        /// Run the specified code inside a single transaction.
        ///
        /// In most cases this is not needed, because all APIs will create their own
        /// transaction automatically. It is useful where you want to run multiple
        /// reactive operations atomically.
         ///
        public static void run(Runnable code) {
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

        internal static void run(Handler<Transaction> code) {
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

        internal static A apply<A> (Lambda1<Transaction, A> code) {
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

        /*
        public void prioritized(Node rank, Handler<Transaction> action) {
            Entry e = new Entry(rank, action);
            prioritizedQ.add(e);
            entries.add(e);
        }
        */

        ///
        /// Add an action to run after all prioritized() actions.
         ///
        public void last(Runnable action) {
            lastQ.Add(action);
        }

        /*
        ///
        /// Add an action to run after all last() actions.
         ///
        public void post(Runnable action) {
            if (postQ == null)
                postQ = new ArrayList<Runnable>();
            postQ.add(action);
        }
        */

        ///
        /// If the priority queue has entries in it when we modify any of the nodes'
        /// ranks, then we need to re-generate it to make sure it's up-to-date.
         ///
        private void checkRegen()
        {
            if (toRegen) {
                toRegen = false;
                prioritizedQ.clear();
                foreach (Entry e in entries)
                    prioritizedQ.Enqueue(e);
            }
        }

        public void close() {
            while (true) {
                checkRegen();
                if (prioritizedQ.isEmpty()) break;
                Entry e = prioritizedQ.Dequeue();
                entries.Remove(e);
                e.action.run(this);
            }
            foreach (Runnable action in lastQ)
                action.run();
            lastQ.Clear();
            if (postQ != null) {
                foreach (Runnable action in postQ)
                    action.run();
                postQ.Clear();
            }
        }
    }
}