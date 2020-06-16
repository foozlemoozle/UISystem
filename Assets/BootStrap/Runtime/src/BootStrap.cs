/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exception = System.Exception;
using com.keg.utils;
using System.Threading.Tasks;

namespace com.keg.bootstrap
{
    public class BootStrap : MonoBehaviour
    {
		public enum SetupStatus
		{
			NotStarted,
			Started,
			Complete,
			Failed
		}

        public static event System.Action onSetup;
        public static event System.Action onSetupFail;

        [SerializeField]
        private int _staggerUpdates = -1;

        protected List<IManager> _managers;

        private SetupStatus _setupStatus = SetupStatus.NotStarted;

		private int _lastIndex = 0;

        private PromiseChain<BootStrap, System.Action<IManager>, System.Action<IManager>> _setupChain
			= new PromiseChain<BootStrap, System.Action<IManager>, System.Action<IManager>>();

		public BootStrap Then( IManager manager )
		{
            _setupChain.Then( manager.Setup, this, OnManagerSetup, OnManagerSetupFail );
            return this;
		}

		public BootStrap And( IManager manager )
		{
            _setupChain.And( manager.Setup, this, OnManagerSetup, OnManagerSetupFail );
            return this;
		}

		public async Task InitManagers()
		{
			_setupStatus = SetupStatus.Started;

            await _setupChain.Exec();

			if( _setupStatus != SetupStatus.Failed )
			{
				_setupStatus = SetupStatus.Complete;
			}
		}

		protected void OnManagerSetup( IManager manager )
		{
			if( _managers == null )
			{
                _managers = new List<IManager>();
			}

            if( _setupStatus != SetupStatus.Failed )
            {
                _managers.Add( manager );
            }
		}

		protected void OnManagerSetupFail( IManager manager )
		{
            _setupStatus = SetupStatus.Failed;
            _managers = new List<IManager>();
            onSetupFail();
		}

		public T GetManager<T>() where T : IManager
		{
            int count = _managers.Count;
			for( int i = 0; i < count; ++i )
			{
				if( _managers[ i ] is T )
				{
                    return (T)_managers[ i ];
				}
			}

            return default( T );
		}

		public IManager GetManager( int index )
		{
            int count = _managers.Count;
			if( index < 0 || index >= count )
			{
                return null;
			}

            return _managers[ index ];
		}

		public T GetManager<T>( int index ) where T : IManager
		{
			return (T)GetManager( index );
		}

        // Update is called once per frame
        private void Update()
        {
			if( _setupStatus != SetupStatus.Complete )
			{
				return;
			}

			if( _staggerUpdates > 0 )
			{
				UpdatePartial();
			}
			else
			{
				UpdateAll();
			}
        }

		private void UpdateAll()
		{
			int count = _managers.Count;
			for( int i = 0; i < count; ++i )
			{
				_managers[ i ].Update();
			}
		}

		private void UpdatePartial()
		{
			int count = _managers.Count;

			for( int i = 0; i < _staggerUpdates; ++i )
			{
				_lastIndex += i;
				if( _lastIndex >= count )
				{
					_lastIndex = 0;
				}

				_managers[ _lastIndex ].Update();
			}
		}

		public void Teardown()
		{
			PromiseChain<System.Action> teardownChain = new PromiseChain<System.Action>();
			int count = _managers.Count;
			for( int  i = count - 1; i >= 0; --i )
			{
				teardownChain.Then( _managers[ i ].Teardown, OnTeardown );
			}

			teardownChain.Exec();
		}

		protected virtual void OnTeardown()
		{

		}

		public void OnDestroy()
		{
			Teardown();
		}
	}
}
