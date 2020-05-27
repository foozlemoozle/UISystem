/**
Created by Kirk George 05/23/2019.!--
Manages the heap for a specific ui layer.!-- 
Requests to here should be forwarded by UIManager.!--
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.keg.uisystem
{
    public class UILayerManager : IHeapManager, IUIContext
    {
        public static event System.Action<UIID> HideDialog = CallbackUtils.NoOp;
        public static event System.Action<UIID> ShowDialog = CallbackUtils.NoOp;

        #region IUIContext
        public IHeapManager parent { get { return _uiManager; } }
        public IHeapManager heapManager => this;
        #endregion

        #region IHeapManager
        public int MIN_SORT { get { return -32000; } }
        public int MAX_SORT { get { return 32000; } }
        private UISortingLayer.Layers _layer;
        public UISortingLayer.Layers LAYER { get { return _layer; } }
        #endregion
        private int _currentSort;

        private UIGroupHeap _uiHeap;
        private Dictionary<UIID, ParamSet> _setupParams;

        private DeviceFrame _layerPrefab;
        private DeviceFrame _layerRoot;

        private UIManager _uiManager;
        private bool _showing = false;

        public UILayerManager()
        {
        }

        public void Setup( DeviceFrame layerPrefab, UIManager uiManager, UISortingLayer.Layers layer )
        {
            _layerPrefab = layerPrefab;
            _uiManager = uiManager;
            _layer = layer;

            _uiHeap = new UIGroupHeap();
            _setupParams = new Dictionary<UIID, ParamSet>( UIID.UIIDComparer.Get() );
            _currentSort = MIN_SORT;
        }

        #region IHeapManager
        public UIHandler<UI> Attach<UI>( Loader<UI> loader, int requiredSortOrders, ParamSet setupParams = null, CullSettings cullSettings = CullSettings.NoCullNoClear ) where UI : UIView
        {
            ActivateLayerIfNeed();

            UIID newId = UIID.Generate( cullSettings );
            _setupParams.Add( newId, setupParams );
            _uiHeap.Add( newId );

            int nextSort = _currentSort + requiredSortOrders;

            return new UIHandler<UI>( loader, this, newId, _currentSort, nextSort )
                .Exec( ActivateCanvasIfNeeded )
                .Exec( AttachToCanvas );
        }

        public bool Remove( UIID id )
        {
            bool removed = _uiHeap.Remove( id );
            if( _uiHeap.IsEmpty() )
            {
                HideCanvasIfNeeded();
            }

            return removed;
        }
        #endregion

        private void ActivateLayerIfNeed()
        {
            if( _layerRoot == null )
            {
                _layerRoot = GameObject.Instantiate<DeviceFrame>( _layerPrefab, _uiManager.transform );
                _layerRoot.canvas.renderMode = RenderMode.ScreenSpaceCamera;
                _layerRoot.canvas.worldCamera = _uiManager.uiCamera;
            }
        }

        private void ActivateCanvasIfNeeded<UI>( UI ui ) where UI : UIView
        {
            ActivateCanvasIfNeeded();
        }

        private void ActivateCanvasIfNeeded()
        {
            if( _showing )
            {
                _layerRoot.gameObject.SetActiveIfNeeded( true );
            }
        }

        private void AttachToCanvas<UI>( UI ui ) where UI : UIView
        {
            _layerRoot.AttachUI( ui );
        }

        public void HideBelow( UIID ui )
        {
            if( ( ui.cullSettings & CullSettings.CullBelow ) != 0 )
            {
                _uiHeap.ForGroupBelow( ui, HideDialog );
            }
        }

        public void ShowBelow( UIID id )
        {
            if( ( id.cullSettings & CullSettings.CullBelow ) != 0 )
            {
                _uiHeap.ForGroupBelow( id, ShowDialog );
            }
        }

        private void HideCanvasIfNeeded()
        {
            if( _layerRoot != null )
            {
                _layerRoot.gameObject.SetActiveIfNeeded( false );
            }
        }

        public void HideLayer()
        {
            _showing = false;
            HideCanvasIfNeeded();
        }

        public void ShowLayer()
        {
            _showing = true;
            ActivateCanvasIfNeeded();
        }

        private class UIGroupHeap
        {
            private List<UIGroup> _groups;

            public UIGroupHeap()
            {
                _groups = new List<UIGroup>();
            }

            public void Add( UIID ui )
            {
                if( ( ui.cullSettings & CullSettings.CullBelow ) != 0 )
                {
                    CreateNewTopGroup( ui );
                }
                else
                {
                    AddToTopGroup( ui );
                }
            }

            public void AddToTopGroup( UIID ui )
            {
                if( _groups.Count > 0 )
                {
                    _groups[ _groups.Count - 1 ].Add( ui );
                }
                else
                {
                    CreateNewTopGroup( ui );
                }
            }

            public void CreateNewTopGroup( UIID ui )
            {
                UIGroup group = new UIGroup();
                group.Add( ui );

                _groups.Add( group );
            }

            public bool Remove( UIID ui )
            {
                //try to remove from the top to bottom.
                //this should be more effecient, since dialogs removed should more likely be on the top
                int count = _groups.Count;
                for( int i = count - 1; i >= 0; --i )
                {
                    if( _groups[ i ].Remove( ui ) )
                    {
                        if( _groups[ i ].IsEmpty() )
                        {
                            _groups.RemoveAt( i );
                        }

                        return true;
                    }
                }

                return false;
            }

            public bool IsEmpty()
            {
                return _groups.Count == 0;
            }

            public void ForGroupBelow( UIID uiid, System.Action<UIID> action )
            {
                if( action == null )
                {
                    return;
                }

                int count = _groups.Count;
                bool readyToExec = false;
                for( int i = count - 1; i >= 0; --i )
                {
                    if( _groups[ i ].Contains( uiid ) )
                    {
                        readyToExec = true;
                        continue;
                    }
                    else if( readyToExec )
                    {
                        int uiidCount = _groups[ i ].Count;
                        for( int k = 0; k < uiidCount; ++k )
                        {
                            action( _groups[ i ][ k ] );
                        }

                        break;
                    }
                }
            }
        }

        private class UIGroup : List<UIID>
        {
            public bool IsEmpty()
            {
                return Count == 0;
            }
        }
    }
}
