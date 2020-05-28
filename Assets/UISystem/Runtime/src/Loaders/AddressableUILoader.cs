/**
 * Loader that utilizes Unity's Addressables system for loading.
 */

using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AddressableAssetsIResourceLocator = UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator;
using ResourceManagementIResourceLocator = UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation;
using com.keg.addressableloadmanagement;

namespace com.keg.uisystem
{
    public class AddressableUILoader<T> : Loader<T>, IAddressableLoader where T : UIView
    {
        public string addressablePath => _path;
        public string addressableGroup => _group;

        private string _path;
        private string _group;

        public AddressableUILoader( string path, string group )
        {
            _path = path;
            _group = group;
        }

        public override void StartLoad()
        {
            AddressableManager.Get().QueueLoader( this );
        }

        public void OnLoadComplete( AsyncOperationHandle<UnityEngine.Object> loaded )
        {
            UnityEngine.GameObject loadedAsset = loaded.Result as UnityEngine.GameObject;

            if( loaded.IsDone && loaded.Status == AsyncOperationStatus.Succeeded )
            {
                var instantiated = UnityEngine.GameObject.Instantiate( loadedAsset );
                OnLoaded( instantiated );
            }
            else
            {
                Reject( loadedAsset );
            }
        }
    }
}

