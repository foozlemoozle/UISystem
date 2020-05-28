using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.keg.uisystem;
using com.keg.bootstrap;

namespace com.keg.uisystem.tests
{
    public class TestRunner : UIService
    {
        private UIHandler<TestHudView> _handler;

        // Update is called once per frame
        void Update()
        {

        }

		protected override void OnSetupFinished( IManager manager )
		{
			base.OnSetupFinished( manager );
            LoadTestDialog( manager as UIManager );
		}

		protected override void OnTeardown()
        {
            base.OnTeardown();

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
