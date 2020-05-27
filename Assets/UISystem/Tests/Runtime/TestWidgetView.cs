using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.keg.uisystem;

namespace com.keg.uisystem.tests
{
    public class TestWidgetView : UIView
    {
        public static readonly string PATH = "Assets/Test/TestWidget.prefab";
        public static readonly string GROUP = "ui_prefab";

        public override uint expectedPostLoadProcesses => 0;
    }
}
