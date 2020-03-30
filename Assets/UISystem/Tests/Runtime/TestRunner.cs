using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRunner : MonoBehaviour
{
    private UIHandler<TestHudView> _handler;

    public void Awake()
    {
        UIManager.ListenOnUIManagerSetup( LoadTestDialog );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
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
