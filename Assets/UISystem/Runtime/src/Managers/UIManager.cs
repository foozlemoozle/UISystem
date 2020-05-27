/**
Created by Kirk George 05/23/2019.!--
Manages all of the UI layers.!--
Initial dialog loads should all go through here.!--
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.keg.uisystem
{
    [RequireComponent( typeof( Camera ) )]
    public class UIManager : MonoBehaviour, IHeapManager
    {
        private static event System.Action<UIManager> _onUIManagerSetup = CallbackUtils.NoOp<UIManager>;
        private static UIManager _instance = null;

        public static void ListenOnUIManagerSetup( System.Action<UIManager> onSetup )
        {
            if( _instance != null )
            {
                onSetup( _instance );
            }
            else
            {
                _onUIManagerSetup += onSetup;
            }
        }

        //For IHeapManager's implementatin of Min and Max sort.
        //Doesn't really mean anything for UIManager.  
        #region IHeapManager
        public int MIN_SORT { get { return 0; } }
        public int MAX_SORT { get { return 0; } }
        public UISortingLayer.Layers LAYER { get { return UISortingLayer.Layers.UIManager; } }
        #endregion

        [SerializeField]
        private bool _setupOnAwake = true;
        [SerializeField]
        [RequiredField]
        private DeviceFrame _layerPrefab;

        private Dictionary<UISortingLayer.Layers, UILayerManager> _layers;
        private Dictionary<UIID, UISortingLayer.Layers> _uiToLayerMap;
        private Dictionary<UISortingLayer.Layers, int> _layerCullRefCount;
        private Camera _uiCamera;
        public Camera uiCamera
        {
            get
            {
                if( _uiCamera == null )
                {
                    _uiCamera = this.GetComponent<Camera>();
                }

                return _uiCamera;
            }
        }

        public void Awake()
        {
            if( _setupOnAwake )
            {
                Setup();
            }
        }

        public void Setup()
        {
            if( _layers == null )
            {
                _layers = new Dictionary<UISortingLayer.Layers, UILayerManager>( UISortingLayer.count, UISortingLayer.LayerComparer.Get() );
                IEnumerator<UISortingLayer.Layers> layerIter = UISortingLayer.GetEnumerator();
                while( layerIter.MoveNext() )
                {
                    _layers.Add( layerIter.Current, GenerateLayer( layerIter.Current ) );
                }
            }
            else
            {
                throw new UIManagerSetupException();
            }

            if( _uiToLayerMap == null )
            {
                _uiToLayerMap = new Dictionary<UIID, UISortingLayer.Layers>( UIID.UIIDComparer.Get() );
            }
            else
            {
                throw new UIManagerSetupException();
            }

            if( _layerCullRefCount == null )
            {
                _layerCullRefCount = new Dictionary<UISortingLayer.Layers, int>( UISortingLayer.count, UISortingLayer.LayerComparer.Get() );
                IEnumerator<UISortingLayer.Layers> layerIter = UISortingLayer.GetEnumerator();
                while( layerIter.MoveNext() )
                {
                    _layerCullRefCount.Add( layerIter.Current, 0 );
                }
            }

            _instance = this;
            _onUIManagerSetup( this );
        }

        private UILayerManager GenerateLayer( UISortingLayer.Layers layer )
        {
            UILayerManager created = new UILayerManager();
            created.Setup( _layerPrefab, this, layer );
            return created;
        }

        #region IHeapManager
        public UIHandler<UI> Attach<UI>( Loader<UI> loader, int requiredSortOrders, ParamSet setupParams = null, CullSettings cullSettings = CullSettings.NoCullNoClear ) where UI : UIView
        {
            return Attach<UI>( loader, UISortingLayer.Layers.Default, requiredSortOrders, setupParams, cullSettings );
        }

        public bool Remove( UIID ui )
        {
            IEnumerator<UISortingLayer.Layers> layerIter = UISortingLayer.GetEnumerator();
            do
            {
                Remove( layerIter.Current, ui );
            }
            while( layerIter.MoveNext() );

            return false;
        }
        #endregion

        public UIHandler<UI> Attach<UI>( Loader<UI> loader, UISortingLayer.Layers uiLayer, int requiredSortOrders, ParamSet setupParams = null, CullSettings cullSettings = CullSettings.NoCullNoClear ) where UI : UIView
        {
            if( _layers.ContainsKey( uiLayer ) )
            {
                UIHandler<UI> handler = _layers[ uiLayer ].Attach<UI>( loader, requiredSortOrders, setupParams, cullSettings )
                    .Exec( UpdateCameraRenderSettings<UI> )
                    .Exec( CheckAndCullLowerUIs<UI> );

                _uiToLayerMap.Add( handler.id, uiLayer );

                return handler;
            }
            else
            {
                throw new UILayerNotFoundException( string.Format( "UIManager: Couldn't find UI layer {0}", uiLayer ) );
            }
        }

        private void UpdateCameraRenderSettings<UI>( UI ui ) where UI : UIView
        {
            CameraClearFlags clearFlags = ( ui.id.cullSettings & CullSettings.ClearCamera ) == 0 ? CameraClearFlags.SolidColor : CameraClearFlags.Depth;
            if( uiCamera.clearFlags != clearFlags )
            {
                uiCamera.clearFlags = clearFlags;
            }
        }

        private void CheckAndCullLowerUIs<UI>( UI ui ) where UI : UIView
        {
            if( ( ui.id.cullSettings & CullSettings.CullBelow ) == 0 )
            {
                return;
            }

            int layer = (int)_uiToLayerMap[ ui.id ];

            _layers[ (UISortingLayer.Layers)layer ].HideBelow( ui.id );

            for( int i = layer - 1; i >= 0; --i )
            {
                _layers[ (UISortingLayer.Layers)i ].HideLayer();
                _layerCullRefCount[ (UISortingLayer.Layers)i ]++;
            }
        }

        public bool Remove( UISortingLayer.Layers layer, UIID uiid )
        {
            CheckAndShowLowerUIs( uiid );
            return _layers[ layer ].Remove( uiid );
        }

        private void CheckAndShowLowerUIs( UIID uiid )
        {
            if( ( uiid.cullSettings & CullSettings.CullBelow ) == 0 )
            {
                return;
            }

            int layer = (int)_uiToLayerMap[ uiid ];

            _layers[ (UISortingLayer.Layers)layer ].ShowBelow( uiid );

            for( int i = layer - 1; i >= 0; --i )
            {
                UISortingLayer.Layers tLayer = (UISortingLayer.Layers)i;

                _layerCullRefCount[ tLayer ]--;
                if( _layerCullRefCount[ tLayer ] <= 0 )
                {
                    _layers[ tLayer ].ShowLayer();
                }
            }
        }
    }
}
