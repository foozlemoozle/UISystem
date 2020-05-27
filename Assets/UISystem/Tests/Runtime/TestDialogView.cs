using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.keg.uisystem;

namespace com.keg.uisystem.tests
{
    public class TestDialogView : UIView
    {
        public static readonly string PATH = "Assets/Test/TestDialog.prefab";
        public static readonly string GROUP = "ui_prefab";

        public override uint expectedPostLoadProcesses => 0;

        public void OnAddDialog()
        {
            AddressableUILoader<TestDialogView> loader = new AddressableUILoader<TestDialogView>( PATH, GROUP );
            this.Attach<TestDialogView>( loader, 1 );
        }

        public void OnRemoveSelf()
        {
            parent.Remove( id );
        }

        public void OnAddWidget()
        {
            AddressableUILoader<TestWidgetView> loader = new AddressableUILoader<TestWidgetView>( TestWidgetView.PATH, TestWidgetView.GROUP );
            this.Attach<TestWidgetView>( loader, 0 );
        }
    }
}
