01) What is a Thread? A Thread is defined as the smallest unit of code to which an operating system allocates CPU time. In multithreading, a single process has multiple threads of execution. If the system has multiple cpu’s then it can run in parallel.
	A thread has its own call stack that stores all the methods that are executed.
	Local variables are stored on the call stack and are private to the thread.
	A thread can also have its own data that’s not a local variable. By marking a field with the ThreadStatic attribute each thread gets its own copy of a field
	If you want to use local data in a thread and initialize it for each thread, you can use the ThreadLocal class
	You can use the Thread.CurrentThread class to ask for information about the current thread that’s executing.
02) What is Thread Pooling? Thread creation consumes time and resources. So you can use Thread pool to reuse the thread once it is created. So when you use the thread pool in .net, all your requests goes to Thread pool and then picked up by an available thread from the pool.
	The thread pool ensures that each request gets added to the queue and that when a thread becomes available, it is processed. The thread pool automatically manages the number of threads it needs to keep around.
03) What concurrent collections are available in .NET?	BlockingCollection<T>, ConcurrentBag<T>, ConcurrentDictionary<TKey,T>, ConcurrentStack<T>, ConcurrentQueue<T>
04) What is the difference between Task.Run() and Task.Factory.StartNew()? Task.Run is recommended, Task.Factory.StartNew gives you ability to specify more options if needed, e.g, Task.Factory.StartNew(..., TaskCreationOptions.LongRunning);
	So, in the .NET Framework 4.5, the new Task.Run method is introduced. This in no way obsoletes Task.Factory.StartNew, but rather should simply be thought of as a quick way to use Task.Factory.StartNew without needing to specify a bunch of parameters. 
	It’s a shortcut. In fact, Task.Run is actually implemented in terms of the same logic used for Task.Factory.StartNew, just passing in some default parameters. When you pass an Action to Task.Run:
05) A hard rule of thumb is the thread that creates a UI element should be the only thread that updates it.