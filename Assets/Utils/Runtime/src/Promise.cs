using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Action = System.Func<System.Threading.Tasks.Task>;

namespace com.keg.utils
{
    public class PromiseChain
    {
		private Queue<Promise> _promises = new Queue<Promise>();
		private Promise _lastAdded;

		public void Then( Action task )
		{
			Promise promise = new Promise();
			promise.And( task );

			_promises.Enqueue( promise );
			_lastAdded = promise;
		}

		public void And( Action task )
		{
			if( _promises.Count == 0 )
			{
				Then( task );
				return;
			}

			_lastAdded.And( task );
		}

		public async Task Exec()
		{
			_lastAdded = null;
			while( _promises.Count > 0 )
			{
				await _promises.Dequeue().Exec();
			}
		}
    }

	public class Promise
	{
		private List<Action> _simulTasks = new List<Action>();

		public void And( Action task )
		{
			_simulTasks.Add( task );
		}

		public Task Exec()
		{
			int count = _simulTasks.Count;
			Task[] tasks = new Task[ count ];
			for( int i = 0; i < count; ++i )
			{
				tasks[ i ] = _simulTasks[ i ].Invoke();
			}

			return Task.WhenAll( tasks );
		}
	}

	public class PromiseChain<T>
	{
		private Queue<Promise<T>> _promises = new Queue<Promise<T>>();
		private Promise<T> _lastAdded;

		public void Then( System.Func<T, Task> task, T param )
		{
			Promise<T> promise = new Promise<T>();
			promise.And( task, param );

			_promises.Enqueue( promise );
			_lastAdded = promise;
		}

		public void And( System.Func<T, Task> task, T param )
		{
			if( _promises.Count == 0 )
			{
				Then( task, param );
				return;
			}

			_lastAdded.And( task, param );
		}

		public async Task Exec()
		{
			_lastAdded = null;
			while( _promises.Count > 0 )
			{
				await _promises.Dequeue().Exec();
			}
		}
	}

	public class Promise<T>
	{
		private List<(System.Func<T, Task> task, T param)> _simulTasks = new List<(System.Func<T, Task> task, T param)>();

		public void And( System.Func<T, Task> task, T param )
		{
			_simulTasks.Add( (task: task, param: param) );
		}

		public Task Exec()
		{
			int count = _simulTasks.Count;
			Task[] tasks = new Task[ count ];
			for( int i = 0; i < count; ++i )
			{
				var tuple = _simulTasks[ i ];
				tasks[ i ] = tuple.task.Invoke( tuple.param );
			}

			return Task.WhenAll( tasks );
		}
	}

	public class PromiseChain<T1, T2>
	{
		private Queue<Promise<T1, T2>> _promises = new Queue<Promise<T1, T2>>();
		private Promise<T1, T2> _lastAdded;

		public void Then( System.Func<T1, T2, Task> task, T1 param1, T2 param2)
		{
			Promise<T1, T2> promise = new Promise<T1, T2>();
			promise.And( task, param1, param2 );

			_promises.Enqueue( promise );
			_lastAdded = promise;
		}

		public void And( System.Func<T1, T2, Task> task, T1 param1, T2 param2 )
		{
			if( _promises.Count == 0 )
			{
				Then( task, param1, param2 );
				return;
			}

			_lastAdded.And( task, param1, param2 );
		}

		public async Task Exec()
		{
			_lastAdded = null;
			while( _promises.Count > 0 )
			{
				await _promises.Dequeue().Exec();
			}
		}
	}

	public class Promise<T1, T2>
	{
		private List<(System.Func<T1, T2, Task> task, T1 param1, T2 param2)> _simulTasks = new List<(System.Func<T1, T2, Task> task, T1 param1, T2 param2)>();

		public void And( System.Func<T1, T2, Task> task, T1 param1, T2 param2 )
		{
			_simulTasks.Add( (task: task, param1: param1, param2: param2) );
		}

		public Task Exec()
		{
			int count = _simulTasks.Count;
			Task[] tasks = new Task[ count ];
			for( int i = 0; i < count; ++i )
			{
				var tuple = _simulTasks[ i ];
				tasks[ i ] = tuple.task.Invoke( tuple.param1, tuple.param2 );
			}

			return Task.WhenAll( tasks );
		}
	}

	public class PromiseChain<T1, T2, T3>
	{
		private Queue<Promise<T1, T2, T3>> _promises = new Queue<Promise<T1, T2, T3>>();
		private Promise<T1, T2, T3> _lastAdded;

		public void Then( System.Func<T1, T2, T3, Task> task, T1 param1, T2 param2, T3 param3 )
		{
			Promise<T1, T2, T3> promise = new Promise<T1, T2, T3>();
			promise.And( task, param1, param2, param3 );

			_promises.Enqueue( promise );
			_lastAdded = promise;
		}

		public void And( System.Func<T1, T2, T3, Task> task, T1 param1, T2 param2, T3 param3 )
		{
			if( _promises.Count == 0 )
			{
				Then( task, param1, param2, param3 );
				return;
			}

			_lastAdded.And( task, param1, param2, param3 );
		}

		public async Task Exec()
		{
			_lastAdded = null;
			while( _promises.Count > 0 )
			{
				await _promises.Dequeue().Exec();
			}
		}
	}

	public class Promise<T1, T2, T3>
	{
		private List<(System.Func<T1, T2, T3, Task> task, T1 param1, T2 param2, T3 param3)> _simulTasks
			= new List<(System.Func<T1, T2, T3, Task> task, T1 param1, T2 param2, T3 param3)>();

		public void And( System.Func<T1, T2, T3, Task> task, T1 param1, T2 param2, T3 param3 )
		{
			_simulTasks.Add( (task: task, param1: param1, param2: param2, param3: param3) );
		}

		public Task Exec()
		{
			int count = _simulTasks.Count;
			Task[] tasks = new Task[ count ];
			for( int i = 0; i < count; ++i )
			{
				var tuple = _simulTasks[ i ];
				tasks[ i ] = tuple.task.Invoke( tuple.param1, tuple.param2, tuple.param3 );
			}

			return Task.WhenAll( tasks );
		}
	}
}
