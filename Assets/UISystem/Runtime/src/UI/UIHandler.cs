/**
Created by Kirk George 05/23/2019.!--
Object returned synchronously from ui attach calls.!--
Allows the user to synchronously access a UI without worrying about any NREs.!--
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.keg.uisystem
{
	public interface IUIHandler
	{
		IUIContext context { get; }
		UIID id { get; }
		void Teardown();
	}

	public class UIHandler<UI> : IUIHandler where UI : UIView
	{
		public IUIContext context { get; private set; }
		public UIID id { get; private set; }
		public UI ui { get; private set; }

		protected int _minSort;
		protected int _maxSort;

		private Loader<UI> _loader;

		private Queue<System.Action<UI>> _postLoadQueue;

		public UIHandler( Loader<UI> loader, IUIContext context, UIID id, int minSort, int maxSort )
		{
			_loader = loader;
			this.context = context;
			this.id = id;
			_minSort = minSort;
			_maxSort = maxSort;

			_postLoadQueue = new Queue<System.Action<UI>>();

			_loader.onAccepted = OnUILoadComplete;
			_loader.onComplete = OnUIPostLoadProcessesComplete;
			_loader.onFailed = OnUILoadFailed;
			_loader.StartLoad();
		}

		public UIHandler<UI> Exec( System.Action<UI> action )
		{
			if( action == null )
			{
				return this;
			}

			if( ui != null )
			{
				action( ui );
			}
			else
			{
				_postLoadQueue.Enqueue( action );
			}

			return this;
		}

		private void OnUILoadComplete()
		{
			ui = _loader.ui;
			ui.Initialize( id, _minSort, _maxSort, context );
		}

		private void OnUIPostLoadProcessesComplete()
		{
			_loader = null;

			while( _postLoadQueue.Count > 0 )
			{
				var exec = _postLoadQueue.Dequeue();
				if( exec != null )
				{
					exec( ui );
				}
			}
		}

		private void OnUILoadFailed()
		{
			_loader = null;
			Teardown();
		}

		public void Teardown()
		{
			_postLoadQueue.Clear();
			context.parent.Remove( id );

			if( _loader != null )
			{
				_loader.Cancel();
				_loader = null;
			}
			if( ui != null )
			{
				GameObject.Destroy( ui.gameObject );
				ui = null;
			}
		}
	}
}
