using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.keg.uisystem;
using com.keg.bootstrap;

namespace com.keg.uisystem.tests
{
    public class TestRunner : MonoBehaviour
    {
        private UIHandler<TestHudView> _handler;
        private UIManager _uiManager;

		private void Awake()
		{
            _uiManager = new UIManager();
            _uiManager.Setup( null, OnSetupFinished, OnSetupFailed );
		}

		// Update is called once per frame
		void Update()
        {

        }

		protected void OnSetupFinished( IManager manager )
		{
            LoadTestDialog( manager as UIManager );
		}

        private void OnSetupFailed( IManager manager )
        {
        }

		private void OnDestroy()
		{
            _uiManager.Teardown( OnTeardown );
		}

		protected void OnTeardown()
        {
            if( _handler != null )
            {
                _handler.Teardown();
            }
        }

        private void LoadTestDialog( UIManager uiManager )
        {
            AddressableUILoader<TestHudView> loader = new AddressableUILoader<TestHudView>( TestHudView.PATH, TestHudView.GROUP );
            _handler = uiManager.Attach<TestHudView>( loader, 1, null, CullSettings.NoCullNoClear );
        }
    }
}
