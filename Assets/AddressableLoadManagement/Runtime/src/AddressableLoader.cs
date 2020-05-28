using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.keg.addressableloadmanagement
{
    public class AddressableLoader<T> : IAddressableLoader where T : UnityEngine.Object
    {
        protected string _addressablePath;
        protected string _addressableGroup;
        protected System.Action<T> _onLoaded;

        public string addressablePath => _addressablePath;
        public string addressableGroup => _addressableGroup;

		public AddressableLoader( string path, string group, System.Action<T> onLoaded )
		{
			_addressablePath = path;
			_addressableGroup = group;
			_onLoaded = onLoaded;
		}

        public void OnLoadComplete( AsyncOperationHandle<UnityEngine.Object> loaded )
		{
			var type = typeof( T );
			T result = null;
			if( type.IsSubclassOf( typeof( UnityEngine.Component ) ) )
			{
				result = ( (UnityEngine.GameObject)loaded.Result ).GetComponent<T>();
			}
			else
			{
				result = loaded.Result as T;
			}
			_onLoaded?.Invoke( result );
		}
    }
}
