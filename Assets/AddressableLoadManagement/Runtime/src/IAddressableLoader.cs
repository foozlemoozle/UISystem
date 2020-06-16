/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.keg.addressableloadmanagement
{
    public interface IAddressableLoader
    {
        string addressablePath { get; }
        string addressableGroup { get; }
        void OnLoadComplete( AsyncOperationHandle<UnityEngine.Object> loaded );
    }

	public static class IAddressableLoaderFactory
	{
		public static void Load<T>( this IAddressableLoader loader, string path, string group, System.Action<T> onLoaded ) where T : UnityEngine.Object
		{
			loader = new AddressableLoader<T>( path, group, onLoaded );
			AddressableManager.Get().QueueLoader( loader );
		}

		public static void Load( this IAddressableLoader loader )
		{
			if( loader == null )
			{
				UnityEngine.Debug.LogError( "<color=red>ERROR:</color> Cannot load null loader without a provided path and group." );
				return;
			}
			AddressableManager.Get().QueueLoader( loader );
		}

		public static void ReleaseAddressable( this UnityEngine.Object toUnload )
		{
			if( toUnload == null )
			{
				return;
			}
			AddressableManager.Get().FreeResource( toUnload );
		}
	}
}
