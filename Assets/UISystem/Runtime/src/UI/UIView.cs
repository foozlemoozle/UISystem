/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

/**
Created by Kirk George 05/23/2019.!--
Component attached to ui gameobject.!--
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace com.keg.uisystem
{
	public abstract class UIView : UIBehaviour, IHeapManager, IUIContext
	{
		public UIID id { get; private set; }
		public IUIContext context { get; private set; }

		[SerializeField]
		private bool _ignoreFrameLayout = false;
		public bool ignoreFrameLayout { get { return _ignoreFrameLayout; } }

		private Canvas _canvas;
		public Canvas canvas
		{
			get
			{
				if( _canvas == null )
				{
					_canvas = this.GetComponent<Canvas>();
				}
				if( _canvas == null )
				{
					_canvas = this.gameObject.AddComponent<Canvas>();
					_canvas.overrideSorting = true;
				}
				if( this.GetComponent<GraphicRaycaster>() == null )
				{
					this.gameObject.AddComponent<GraphicRaycaster>();
				}

				return _canvas;
			}
		}

		protected Dictionary<UIID, IUIHandler> _managedUIs;
		protected Dictionary<UIID, ParamSet> _managedUIsSetupParams;

		#region IUIContext
		public IHeapManager parent => _parent;
		private IHeapManager _parent = null;
		public IHeapManager heapManager => this;
		#endregion

		#region IHeapManager
		public int MIN_SORT { get; private set; }
		public int MAX_SORT { get; private set; }
		public UISortingLayer.Layers LAYER { get { return parent.LAYER; } }
		#endregion

		#region LoadSequence
		public abstract uint expectedPostLoadProcesses { get; }
		protected uint completedPostLoadProcesses = 0;
		private System.Action _onPostLoadProcessesComplete;

		public void Initialize( UIID id, int minSort, int maxSort, IUIContext context, ParamSet setupParams )
		{
			this.id = id;
			this.context = context;
			_parent = context.heapManager;
			MIN_SORT = minSort;
			MAX_SORT = maxSort;

			canvas.sortingOrder = MIN_SORT;
			canvas.sortingLayerName = this.context.parent.LAYER.ToString();

			PostInitialize( setupParams );
		}

		protected virtual void PostInitialize( ParamSet setupParams )
		{
		}

		public void StartPostLoadProcesses( System.Action onPostLoadProcessesComplete )
		{
			_onPostLoadProcessesComplete = onPostLoadProcessesComplete;
			completedPostLoadProcesses = 0;

			PreStartPostLoadProcesses();

			if( AllPostLoadProcessesComplete() )
			{
				PostLoadProcessesComplete();
			}
		}

		protected virtual void PreStartPostLoadProcesses()
		{
		}

		public void Update()
		{
			if( _onPostLoadProcessesComplete != null && AllPostLoadProcessesComplete() )
			{
				PostLoadProcessesComplete();
			}
		}

		public bool AllPostLoadProcessesComplete()
		{
			return expectedPostLoadProcesses == completedPostLoadProcesses;
		}

		private void PostLoadProcessesComplete()
		{
			if( _onPostLoadProcessesComplete != null )
			{
				_onPostLoadProcessesComplete();
				_onPostLoadProcessesComplete = null;
			}
		}
		#endregion

		#region IHeapManager
		public UIHandler<UI> Attach<UI>( Loader<UI> loader, int requiredSortOrders, ParamSet setupParams = null, CullSettings cullSettings = CullSettings.NoCullNoClear ) where UI : UIView
		{
			CheckAndInitializeChildUIManagement();

			UIID newId = UIID.Generate( cullSettings );
			_managedUIsSetupParams.Add( newId, setupParams );

			int minSort = GetMinSortForChildUI();
			int maxSort = minSort + requiredSortOrders;

			UpdateMinSortForNextChildUI( maxSort );

			UIHandler<UI> handler = new UIHandler<UI>( loader, this, newId, minSort, maxSort, setupParams )
				.Exec( SetParent );

			_managedUIs.Add( newId, handler );

			return handler;
		}

		public bool Remove( UIID id )
		{
			if( _managedUIs.ContainsKey( id ) )
			{
				IUIHandler handler = _managedUIs[ id ];

				_managedUIsSetupParams.Remove( id );
				_managedUIs.Remove( id );

				handler.Teardown();

				return true;
			}

			return false;
		}
		#endregion

		private void CheckAndInitializeChildUIManagement()
		{
			if( _managedUIs == null )
			{
				_managedUIs = new Dictionary<UIID, IUIHandler>();
			}
			if( _managedUIsSetupParams == null )
			{
				_managedUIsSetupParams = new Dictionary<UIID, ParamSet>();
			}
		}

		protected virtual int GetMinSortForChildUI()
		{
			return MIN_SORT;
		}

		protected virtual void UpdateMinSortForNextChildUI( int maxSort )
		{
			//this does nothing at a base level
			//this should be overwritten for more specific sub-ui management
		}

		protected virtual void SetParent<UI>( UI ui ) where UI : UIView
		{
			//by default, just child it to this
			ui.transform.SetParent( this.transform, false );
		}
	}
}
