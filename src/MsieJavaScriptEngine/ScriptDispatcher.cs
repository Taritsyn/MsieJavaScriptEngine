﻿using System;
using System.Collections.Generic;
using System.Threading;

using MsieJavaScriptEngine.Utilities;

namespace MsieJavaScriptEngine
{
	/// <summary>
	/// Provides services for managing the queue of script tasks on the thread with increased stack size
	/// </summary>
	internal sealed class ScriptDispatcher : IDisposable
	{
#if !NETSTANDARD1_3
		/// <summary>
		/// The stack size is sufficient to run the code of modern JavaScript libraries in 32-bit process
		/// </summary>
		const int STACK_SIZE_32 = 492 * 1024; // like 32-bit Node.js

		/// <summary>
		/// The stack size is sufficient to run the code of modern JavaScript libraries in 64-bit process
		/// </summary>
		const int STACK_SIZE_64 = 984 * 1024; // like 64-bit Node.js

#endif
		/// <summary>
		/// The thread with increased stack size
		/// </summary>
		private Thread _thread;

		/// <summary>
		/// Event to signal when the new script task entered to the queue
		/// </summary>
		private AutoResetEvent _waitHandle = new AutoResetEvent(false);

		/// <summary>
		/// Queue of script tasks
		/// </summary>
		private readonly Queue<ScriptTask> _taskQueue = new Queue<ScriptTask>();

		/// <summary>
		/// Synchronizer of script task queue
		/// </summary>
		private readonly object _taskQueueSynchronizer = new object();

		/// <summary>
		/// Flag that object is destroyed
		/// </summary>
		private InterlockedStatedFlag _disposedFlag = new InterlockedStatedFlag();


		/// <summary>
		/// Constructs an instance of script dispatcher
		/// </summary>
		public ScriptDispatcher()
		{
#if NETSTANDARD1_3
			_thread = new Thread(StartThread)
#else
			int sufficientStackSize = Utils.Is64BitProcess() ? STACK_SIZE_64 : STACK_SIZE_32;

			_thread = new Thread(StartThread, sufficientStackSize)
#endif
			{
				IsBackground = true
			};
			_thread.Start();
		}


		private void VerifyNotDisposed()
		{
			if (_disposedFlag.IsSet())
			{
				throw new ObjectDisposedException(ToString());
			}
		}

		/// <summary>
		/// Starts a thread with increased stack size.
		/// Loops forever, processing script tasks from the queue.
		/// </summary>
		private void StartThread()
		{
			while(true)
			{
				ScriptTask task = null;

				lock (_taskQueueSynchronizer)
				{
					if (_taskQueue.Count > 0)
					{
						task = _taskQueue.Dequeue();
						if (task == null)
						{
							_taskQueue.Clear();
							return;
						}
					}
				}

				if (task != null)
				{
					try
					{
						task.Result = task.Delegate();
					}
					catch (Exception e)
					{
						task.Exception = e;
					}

					task.WaitHandle.Set();
				}
				else
				{
					_waitHandle.WaitOne();
				}
			}
		}

		/// <summary>
		/// Adds a script task to the end of the queue
		/// </summary>
		/// <param name="task">Script task</param>
		private void EnqueueTask(ScriptTask task)
		{
			lock (_taskQueueSynchronizer)
			{
				_taskQueue.Enqueue(task);
			}
			_waitHandle.Set();
		}

		/// <summary>
		/// Runs a specified delegate on the thread with increased stack size,
		/// and returns its result as an <see cref="System.Object"/>.
		/// Blocks until the invocation of delegate is completed.
		/// </summary>
		/// <param name="del">Delegate to invocation</param>
		/// <returns>Result of the delegate invocation</returns>
		private object InnnerInvoke(Func<object> del)
		{
			ScriptTask task;

			using (var waitHandle = new ManualResetEvent(false))
			{
				task = new ScriptTask(del, waitHandle);
				EnqueueTask(task);

				waitHandle.WaitOne();
			}

			if (task.Exception != null)
			{
				throw task.Exception;
			}

			return task.Result;
		}

		/// <summary>
		/// Runs a specified delegate on the thread with increased stack size,
		/// and returns its result as an <typeparamref name="T" />.
		/// Blocks until the invocation of delegate is completed.
		/// </summary>
		/// <typeparam name="T">The type of the return value of the method,
		/// that specified delegate encapsulates</typeparam>
		/// <param name="func">Delegate to invocation</param>
		/// <returns>Result of the delegate invocation</returns>
		public T Invoke<T>(Func<T> func)
		{
			VerifyNotDisposed();

			if (func == null)
			{
				throw new ArgumentNullException("func");
			}

			return (T)InnnerInvoke(() => func());
		}

		/// <summary>
		/// Runs a specified delegate on the thread with increased stack size.
		/// Blocks until the invocation of delegate is completed.
		/// </summary>
		/// <param name="action">Delegate to invocation</param>
		public void Invoke(Action action)
		{
			VerifyNotDisposed();

			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			InnnerInvoke(() =>
			{
				action();
				return null;
			});
		}

		#region IDisposable implementation

		/// <summary>
		/// Destroys object
		/// </summary>
		public void Dispose()
		{
			if (_disposedFlag.Set())
			{
				EnqueueTask(null);

				if (_thread != null)
				{
					_thread.Join();
					_thread = null;
				}

				if (_waitHandle != null)
				{
					_waitHandle.Dispose();
					_waitHandle = null;
				}
			}
		}

		#endregion

		#region Internal types

		/// <summary>
		/// Represents a script task, that must be executed on separate thread
		/// </summary>
		private sealed class ScriptTask
		{
			/// <summary>
			/// Gets a delegate to invocation
			/// </summary>
			public Func<object> Delegate
			{
				get;
				private set;
			}

			/// <summary>
			/// Gets a event to signal when the invocation of delegate has completed
			/// </summary>
			public ManualResetEvent WaitHandle
			{
				get;
				private set;
			}

			/// <summary>
			/// Gets or sets a result of the delegate invocation
			/// </summary>
			public object Result
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets a exception, that occurred during the invocation of delegate.
			/// If no exception has occurred, this will be null.
			/// </summary>
			public Exception Exception
			{
				get;
				set;
			}


			/// <summary>
			/// Constructs an instance of script task
			/// </summary>
			/// <param name="del">Delegate to invocation</param>
			/// <param name="waitHandle">Event to signal when the invocation of delegate has completed</param>
			public ScriptTask(Func<object> del, ManualResetEvent waitHandle)
			{
				Delegate = del;
				WaitHandle = waitHandle;
			}
		}

		#endregion
	}
}