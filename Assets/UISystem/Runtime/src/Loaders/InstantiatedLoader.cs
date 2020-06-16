/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

using UnityEngine;

namespace com.keg.uisystem
{
    public class InstantiatedLoader<T> : Loader<T> where T : UIView
    {
        private T _instantiated;

		public InstantiatedLoader( T instance )
		{
			_instantiated = instance;
		}

		public override void StartLoad()
		{
			if( _instantiated != null )
			{
				OnLoaded( _instantiated.gameObject );
			}
			else
			{
				Reject( null );
			}
		}
	}
}
