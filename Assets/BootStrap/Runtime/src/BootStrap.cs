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
        public static event System.Action onSetup;
        public static event System.Action onSetupFail;

        [SerializeField]
        private int _staggerUpdates = -1;

        protected List<IManager> _managers;

        private bool _setupFailed = false;

		private int _lastIndex = 0;

        private PromiseChain<BootStrap, System.Action<IManager>, System.Action<IManager>> _setupChain
			= new PromiseChain<BootStrap, System.Action<IManager>, System.Action<IManager>>();

        // Start is called before the first frame update
        void Start()
        {

        }

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

		public void InitManagers()
		{
            _setupChain.Exec();
		}

		protected void OnManagerSetup( IManager manager )
		{
			if( _managers == null )
			{
                _managers = new List<IManager>();
			}

            if( !_setupFailed )
            {
                _managers.Add( manager );
            }
		}

		protected void OnManagerSetupFail( IManager manager )
		{
            _setupFailed = true;
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

		protected void OnTeardown()
		{

		}

		public void OnDestroy()
		{
			Teardown();
		}
	}
}
