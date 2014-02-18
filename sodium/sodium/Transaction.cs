namespace sodium
{
    using System;
    using System.Collections.Generic;

    public sealed class Transaction
    {
        // Coarse-grained lock that's held during the whole transaction.
        private static readonly Object TransactionLock = new Object();

        // Fine-grained lock that protects listeners and nodes.
        public static readonly Object ListenersLock = new Object();

        public bool NeedToRegeneratePriorityQueue = false;

        private readonly IPriorityQueue<Entry> _prioritized = new PriorityQueue<Entry>();
        private readonly ISet<Entry> _entries = new HashSet<Entry>();
        private readonly List<IRunnable> _last = new List<IRunnable>();
        private List<IRunnable> _post = new List<IRunnable>();
        private static Transaction _currentTransaction;

        /// <summary>
        /// Run the specified code inside a single transaction.
        ///
        /// In most cases this is not needed, because all APIs will create their own
        /// transaction automatically. It is useful where you want to run multiple
        /// reactive operations atomically.
        /// </summary>
        /// <param name="code"></param>
        public static void Run(IRunnable code)
        {
            Run(new Handler<Transaction>(t => code.Run()));
        }

        public static void Run(Action<Transaction> code)
        {
            Run(new Handler<Transaction>(code));
        }

        public static void Run(IHandler<Transaction> code)
        {
            lock (TransactionLock)
            {
                // If we are already inside a transaction (which must be on the same
                // thread otherwise we wouldn't have acquired transactionLock), then
                // keep using that same transaction.
                var previousTransaction = _currentTransaction;
                try
                {
                    if (_currentTransaction == null)
                    { 
                        _currentTransaction = new Transaction();
                    }
                    code.Run(_currentTransaction);
                }
                finally
                {
                    if (previousTransaction == null)
                    { 
                        _currentTransaction.Close();
                    }
                    _currentTransaction = previousTransaction;
                }
            }
        }

        public static TResult Apply<TResult>(IFunction<Transaction, TResult> code)
        {
            lock (TransactionLock)
            {
                // If we are already inside a transaction (which must be on the same
                // thread otherwise we wouldn't have acquired transactionLock), then
                // keep using that same transaction.
                var transWas = _currentTransaction;
                try
                {
                    if (_currentTransaction == null)
                    { 
                        _currentTransaction = new Transaction();
                    }
                    return code.Apply(_currentTransaction);
                }
                finally
                {
                    _currentTransaction.Close();
                    _currentTransaction = transWas;
                }
            }
        }

        public void Prioritized(Node rank, Action<Transaction> action)
        {
            Prioritized(rank, new Handler<Transaction>(action));
        }

        public void Prioritized(Node rank, IHandler<Transaction> action)
        {
            var e = new Entry(rank, action);
            _prioritized.Add(e);
            _entries.Add(e);
        }

        /// <summary>
        /// Add an action to run after all Prioritized() actions.
        /// </summary>
        /// <param name="action"></param>
        public void Last(IRunnable action)
        {
            _last.Add(action);
        }
        
        /// <summary>
        /// Add an action to run after all Last() actions.
        /// </summary>
        /// <param name="action"></param>
        public void Post(IRunnable action)
        {
            _post.Add(action);
        }

        /// <summary>
        /// If the priority queue has entries in it when we modify any of the nodes'
        /// ranks, then we need to re-generate it to make sure it's up-to-date.
        /// </summary>
        private void CheckRegeneratePriorityQueue()
        {
            if (NeedToRegeneratePriorityQueue)
            {
                RegeneratePriorityQueue();
                NeedToRegeneratePriorityQueue = false;
            }
        }

        private void RegeneratePriorityQueue()
        {
            _prioritized.Clear();
            foreach (var e in _entries)
            { 
                _prioritized.Add(e);
            }
        }

        public void Close()
        {
            while (true)
            {
                CheckRegeneratePriorityQueue();
                if (_prioritized.IsEmpty())
                { 
                    break;
                }
                var e = _prioritized.Remove();
                _entries.Remove(e);
                e.Action.Run(this);
            }

            foreach (var action in _last)
            { 
                action.Run();
            }
            
            _last.Clear();
            
            foreach (var action in _post)
            { 
                action.Run();
            }
            _post.Clear();
        }
    }
}