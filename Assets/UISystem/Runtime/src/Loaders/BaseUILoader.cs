/**
Created by Kirk George 05/23/2019.!--
Handles loading a UI.!--
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.keg.addressableloadmanagement;

namespace com.keg.uisystem
{
	public abstract class BaseLoader
	{
		public enum LoadStep
		{
			//asset failed to load
			Failed = -2,
			//asset load canceled
			Canceled = -1,
			//load not yet started
			NotStarted = 0,
			//waiting for asset to load
			AssetLoading = 1,
			//waiting for loaded asset to load any additional dependencies
			AssetPostLoad = 2,
			//asset loaded successfully
			Complete = 3

		}

		protected LoadStep _current;
		public LoadStep current { get { return _current; } }
		public bool isLoadProcessComplete { get { return _current == LoadStep.Complete; } }
		public bool isAssetLoaded { get { return _current > LoadStep.AssetLoading; } }

		public System.Action onAccepted;
		public System.Action onComplete;
		public System.Action onFailed;
		public System.Action onAborted;

		public abstract void StartLoad();
		protected abstract void OnLoaded( GameObject ui );
		protected abstract void Accept();
		protected abstract void Reject( GameObject ui );
		protected abstract void OnPostLoadProcessesComplete();
		public abstract void Cancel();

		public void Teardown()
		{
			onComplete = null;
			onFailed = null;
			onAborted = null;

			PostTeardown();
		}

		protected abstract void PostTeardown();
	}

	public abstract class Loader<T> : BaseLoader where T : UIView
	{
		private T _ui;
		public T ui { get { return _ui; } }

		//StartLoad needs to be implemented by inhereter
		//Cancel needs to be implemented by inhereter

		protected override sealed void OnLoaded( GameObject ui )
		{
			_ui = ui.GetComponent<T>();
			if( _ui != null )
			{
				Accept();
			}
			else
			{
				Reject( ui );
			}
		}

		protected override sealed void Accept()
		{
			if( _current == LoadStep.Canceled )
			{
				Cancel();
			}
			else
			{
				_current = LoadStep.AssetPostLoad;

				if( onAccepted != null )
				{
					onAccepted();
				}

				_ui.StartPostLoadProcesses( OnPostLoadProcessesComplete );
			}
		}

		protected override sealed void Reject( GameObject ui )
		{
			_ui = null;
			_current = LoadStep.Failed;

			if( ui != null )
			{
				ui.ReleaseAddressable();
			}

			if( onFailed != null )
			{
				onFailed();
			}

			Teardown();
		}

		protected override sealed void OnPostLoadProcessesComplete()
		{
			if( _current == LoadStep.Canceled )
			{
				Cancel();
			}
			else
			{
				_current = LoadStep.Complete;

				if( onComplete != null )
				{
					onComplete();
				}

				Teardown();
			}
		}

		public override sealed void Cancel()
		{
			_current = LoadStep.Canceled;

			if( _ui != null )
			{
				_ui.gameObject.ReleaseAddressable();
			}

			if( onAborted != null )
			{
				onAborted();
			}

			Teardown();
		}

		protected override void PostTeardown()
		{
			_ui = null;
		}
	}
}
