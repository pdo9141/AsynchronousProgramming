using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingSamples
{
    class Program
    {
        public static void ThreadMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadCount: {0}", i);
                Thread.Sleep(0);
            }
        }

        static void Main(string[] args)
        {
            //TestDefault();
            //TestBackground();
            //TestStop();
            //TestThreadPooling();
            //TestBlockingCollection();
            //TestConcurrentBag();
            //TestConcurrentDictionary();
            //TestConcurrentStack();
            //TestConcurrentQueue();
            //TestParallelFor();
            //TestCallMethodWithoutWaitingForReturn();
            //TestPLINQ();
            //TestCancelTask();
            //TestCancelTask2();
            //TestAsyncAwait();
            //TestThreadSynchronization();
            //TestDeadLock();
            TestReaderWriter();
        }

        const int MaxValues = 25;
        static int[] _array = new int[MaxValues];
        static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public static void TestReaderWriter()
        {
            ThreadPool.QueueUserWorkItem(WriteThread);
            for (int i = 0; i < 3; i++)
            {
                ThreadPool.QueueUserWorkItem(ReadThread);
            }
            Console.ReadKey();
        }

        static void WriteThread(object state)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            for (int i = 0; i < MaxValues; ++i)
            {
                _lock.EnterWriteLock();
                Console.WriteLine("Entered WriteLock on thread {0}", id);
                _array[i] = i * i;
                Console.WriteLine("Added {0} to array on thread {1}", _array[i], id);
                Console.WriteLine("Exiting WriteLock on thread {0}", id);
                _lock.ExitWriteLock();
                Thread.Sleep(1000);
            }
        }
        static void ReadThread(object state)
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            for (int i = 0; i < MaxValues; ++i)
            {
                _lock.EnterReadLock();
                Console.WriteLine("Entered ReadLock on thread {0}", id);
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < i; j++)
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(_array[j]);
                }
                Console.WriteLine("Array: {0} on thread {1}", sb, id);
                Console.WriteLine("Exiting ReadLock on thread {0}", id);
                _lock.ExitReadLock();
                Thread.Sleep(1000);
            }
        }

        public static void TestDeadLock()
        {
            object lockA = new object();
            object lockB = new object();
            var up = Task.Run(() =>
            {
                lock (lockA)
                {
                    Thread.Sleep(1000);
                    lock (lockB)
                    {
                        Console.WriteLine("Locked A and B");
                    }
                }
            });
            lock (lockB)
            {
                lock (lockA)
                {
                    Console.WriteLine("Locked A and B");
                }
            }
            up.Wait();
        }

        public static void TestThreadSynchronization()
        {
            int n = 0;
            object _lock = new object();
            var up = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                    lock (_lock)
                        n++;
            });
            for (int i = 0; i < 10; i++)
                lock (_lock)
                    n--;
            up.Wait();
            Console.WriteLine(n);
        }

        public static void TestAsyncAwait()
        {
            var testThread = new AsyncAwaitExample();
            testThread.DoWork();

            while (true)
            {
                Console.WriteLine("Doing work on the Main Thread !!");
            }
        }

        public class AsyncAwaitExample
        {
            public async Task DoWork()
            {
                await Task.Run(() => {
                    int counter;

                    for (counter = 0; counter < 1000; counter++)
                    {
                        Console.WriteLine(counter);
                    }
                });
            }

        }

        public static void TestCancelTask2()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            Task task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.Write("*");
                    Thread.Sleep(1000);
                }
                token.ThrowIfCancellationRequested();
            }, token);
            try
            {
                Console.WriteLine("Press enter to stop the task");
                Console.ReadLine();
                cancellationTokenSource.Cancel();
                task.Wait();
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.InnerExceptions[0].Message);
            }
            Console.WriteLine("Press enter to end the application");
            Console.ReadLine();
        }

        public static void TestDefault()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.Start();
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Main thread is doing its work");
                Thread.Sleep(0);
            }
            t.Join();

            //The Thread.join() method is called on the main thread to let it wait until other thread finishes.
            //The Thread.Sleep() method is used to give a signal to windows that the thread execution is completed.
        }

        public static void TestBackground()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.IsBackground = true;
            t.Start();

            //The application exits immediately, If you run this application with the IsBackground property set to true.
            //If you set it to false, the application prints the ThreadCount message ten times.
        }

        public static void TestStop()
        {
            bool stopped = false;
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (!stopped)
                {
                    Console.WriteLine("Running...");
                    Thread.Sleep(1000);
                }
            }));

            t.Start();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            stopped = true;
            t.Join();
        }

        public static void TestThreadPooling()
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                Console.WriteLine("Working on a thread from threadpool");
            });
            Console.ReadLine();
        }

        public static void TestBlockingCollection()
        {
            BlockingCollection<string> col = new BlockingCollection<string>();
            Task read = Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine(col.Take());
                }
            });
            Task write = Task.Run(() =>
            {
                while (true)
                {
                    string s = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(s)) break;
                    col.Add(s);
                }
            });
            write.Wait();
        }

        public static void TestConcurrentBag()
        {
            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            Task.Run(() =>
            {
                bag.Add(53);
                Thread.Sleep(1000);
                bag.Add(12);
            });
            Task.Run(() =>
            {
                foreach (int i in bag)
                    Console.WriteLine(i);
            }).Wait();
        }

        public static void TestConcurrentDictionary()
        {
            var dict = new ConcurrentDictionary<string, int>();
            if (dict.TryAdd("k1", 53))
            {
                Console.WriteLine("Added");
            }
            if (dict.TryUpdate("k1", 12, 53))
            {
                Console.WriteLine("53 updated to 12");
            }
            dict["k1"] = 53; // Overwrite unconditionally
            int r1 = dict.AddOrUpdate("k1", 3, (s, i) => i * 2);
            int r2 = dict.GetOrAdd("k2", 3);
        }

        public static void TestConcurrentStack()
        {
            ConcurrentStack<int> stack = new ConcurrentStack<int>();
            stack.Push(53);
            int result;
            if (stack.TryPop(out result))
                Console.WriteLine("Popped: {0}", result);
            stack.PushRange(new int[] { 1, 2, 3 });
            int[] values = new int[2];
            stack.TryPopRange(values);
            foreach (int i in values)
                Console.WriteLine(i);
        }

        public static void TestConcurrentQueue()
        {
            ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
            queue.Enqueue(53);
            int result;
            if (queue.TryDequeue(out result))
                Console.WriteLine("Dequeued: {0}", result);
        }

        public static void TestParallelFor()
        {
            int maxPrimes = 1000000;
            int maxNumber = 20000000;
            long primesFound = 0;
            Console.WriteLine("Iterative");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (UInt32 i = 0; i < maxNumber; ++i)
            {
                if (IsPrime(i))
                {
                    Interlocked.Increment(ref primesFound);
                    if (primesFound > maxPrimes)
                    {
                        Console.WriteLine("Last prime found: {0:N0}", i);
                        break;
                    }
                }
            }
            watch.Stop();
            Console.WriteLine("Found {0:N0} primes in {1}", primesFound, watch.Elapsed);
            watch.Reset();
            primesFound = 0;
            Console.WriteLine("Parallel");
            watch.Start();
            //in order to stop the loop, there is an
            //overload that takes Action<int, ParallelLoopState>
            Parallel.For(0, maxNumber, (i, loopState) =>
            {
                if (IsPrime((UInt32)i))
                {
                    Interlocked.Increment(ref primesFound);
                    if (primesFound > maxPrimes)
                    {
                        Console.WriteLine("Last prime found: {0:N0}", i);
                        loopState.Stop();
                    }
                }
            });
            watch.Stop();
            Console.WriteLine("Found {0:N0} primes in {1}", primesFound, watch.Elapsed);
            Console.ReadKey();
        }

        public static bool IsPrime(UInt32 number)
        {
            //check for evenness
            if (number % 2 == 0)
            {
                if (number == 2)
                    return true;
                return false;
            }
            //don’t need to check past the square root
            UInt32 max = (UInt32)Math.Sqrt(number);
            for (UInt32 i = 3; i <= max; i += 2)
            {
                if ((number % i) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        // async method calls must be done through a delegate
        delegate double DoWorkDelegate(int maxValue);

        public static void TestCallMethodWithoutWaitingForReturn()
        {
            DoWorkDelegate del = DoWork;
            //two ways to be notified of when method ends:
            // 1. callback method
            // 2. call EndInvoke
            IAsyncResult res = del.BeginInvoke(100000000, DoWorkDone, null);
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Doing other work...{0}", i);
                Thread.Sleep(1000);
            }
            //wait for end
            double sum = del.EndInvoke(res);
            Console.WriteLine("Sum: {0}", sum);
        }

        static double DoWork(int maxValue)
        {
            Console.WriteLine("In DoWork");
            double sum = 0.0;
            for (int i = 1; i < maxValue; ++i)
            {
                sum += Math.Sqrt(i);
            }
            return sum;
        }

        static void DoWorkDone(object state)
        {
            //didn’t pass in any state
            Console.WriteLine("Computation done");
        }

        public static void TestPLINQ()
        {
            for (var i = 1; i <= 1; i++)
            {
                var myRange = Enumerable.Range(1, 1000000);

                Console.WriteLine("Processing..");

                var stopwatch = Stopwatch.StartNew();

                var result = myRange.Select(x => x);

                stopwatch.Stop();

                Console.WriteLine("Time: {0:FFFFFFF}", stopwatch.Elapsed);

                myRange = null;
                result = null;
            }

            Console.WriteLine();

            Console.WriteLine("Parallel Processing..");

            for (var i = 1; i <= Environment.ProcessorCount; i++)
            {
                var myRange = Enumerable.Range(1, 1000000);

                var stopWatch = Stopwatch.StartNew();

                var result = myRange.AsParallel()
                        .WithDegreeOfParallelism(i)
                        .Select(x => x);

                stopWatch.Stop();

                Console.WriteLine("Number of cores: {0} Time: {1:FFFFFFF}", i, stopWatch.Elapsed);

                myRange = null;
                result = null;
            }

            Console.WriteLine();

            Console.WriteLine("Processing and calling .ToList()");

            for (var i = 1; i <= Environment.ProcessorCount; i++)
            {
                var myRange = Enumerable.Range(1, 1000000);

                var stopWatch = Stopwatch.StartNew();

                var result = myRange.AsParallel()
                       .WithDegreeOfParallelism(i)
                       .Select(x => x).ToList();

                stopWatch.Stop();

                Console.WriteLine("Number of cores: {0} Time: {1:FFFFFFF}", i, stopWatch.Elapsed);

                myRange = null;
                result = null;
            }

            Console.WriteLine();

            Console.WriteLine("Processing and calling .ToList() after PLINQ execution");

            for (var i = 1; i <= Environment.ProcessorCount; i++)
            {
                var myRange = Enumerable.Range(1, 1000000);

                var stopWatch = Stopwatch.StartNew();

                var result = myRange.AsParallel()
                       .WithDegreeOfParallelism(i)
                       .Select(x => x);

                result.ToList();

                stopWatch.Stop();

                Console.WriteLine("Number of cores: {0} Time: {1:FFFFFFF}", i, stopWatch.Elapsed);

                myRange = null;
                result = null;
            }
            
            Console.ReadKey();
        }

        public static void TestCancelTask()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            Task task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.Write("*");
                    Thread.Sleep(1000);
                }
            }, token);
            Console.WriteLine("Press enter to stop the task");
            Console.ReadLine();
            cancellationTokenSource.Cancel();
            Console.WriteLine("Press enter to end the application");
            Console.ReadLine();
        }
    }
}
