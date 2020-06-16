/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

using UnityEngine;

namespace com.keg.uisystem
{
    public class PrefabLoader<T> : Loader<T> where T : UIView
    {
		private T _prefab;

		public PrefabLoader( T prefab )
		{
			_prefab = prefab;
		}

		public override void StartLoad()
		{
			T instantiated = GameObject.Instantiate<T>( _prefab );
			if( instantiated != null )
			{
				OnLoaded( instantiated.gameObject );
			}
			else
			{
				Reject( null );
			}
		}
	}
}
