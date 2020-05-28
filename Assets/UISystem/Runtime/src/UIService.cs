/**
 * Created by Kirk George
 * Monobehaviour runner for UISystem.
 * Use this if you don't want to do the bootstrap paradigm.
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.keg.bootstrap;

namespace com.keg.uisystem
{
    public class UIService : MonoBehaviour
    {
        private static event System.Action<UIManager> _onUIManagerSetup = CallbackUtils.NoOp<UIManager>;
        private static UIService _instance = null;

        public static void ListenOnUIManagerSetup( System.Action<UIManager> onSetup )
        {
            if( _instance != null )
            {
                _onUIManagerSetup( _instance._uiManager );
            }
            else
            {
                _onUIManagerSetup += onSetup;
            }
        }

        [SerializeField]
        private bool _setupOnAwake = true;
        [SerializeField]
        [RequiredField]
        private DeviceFrame _layerPrefab;
        public DeviceFrame layerPrefab => _layerPrefab;

        private UIManager _uiManager;
        public UIManager UIManager => _uiManager;

		public void Awake()
		{
			if( _setupOnAwake )
			{
                Setup();
			}
		}

		public void Setup()
		{
            var manager = new UIManager( layerPrefab, this.transform );
            manager.Setup( null, OnSetupFinished, OnSetupFailed );
		}

		public void OnDestroy()
		{
            _uiManager.Teardown( OnTeardown );
		}

		protected virtual void OnSetupFinished( IManager manager )
		{
            _uiManager = manager as UIManager;
		}

		private void OnSetupFailed( IManager manager )
		{
		}

		protected virtual void OnTeardown()
		{
            _uiManager = null;
		}
    }
}
