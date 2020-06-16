/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.keg.uisystem;

namespace com.keg.uisystem.tests
{
    public class TestHudView : UIView
    {
        public static readonly string PATH = "Assets/UISystem/Tests/Runtime/TestHud.prefab";
        public static readonly string GROUP = "ui_prefab";

        public override uint expectedPostLoadProcesses => 0;

        private Stack<IUIHandler> _toRemove = new Stack<IUIHandler>();

        public void OnAddDialog()
        {
            AddressableUILoader<TestDialogView> loader = new AddressableUILoader<TestDialogView>( TestDialogView.PATH, TestDialogView.GROUP );
            _toRemove.Push( parent.Attach<TestDialogView>( loader, 1 ) );
        }

        public void OnRemoveDialog()
        {
            _toRemove.Pop().Teardown();
        }
    }
}
