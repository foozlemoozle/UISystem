using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AddressableAssetsIResourceLocator = UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator;
using ResourceManagementIResourceLocator = UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation;
using GroupAsyncHandler = UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>>;
using AssetAsyncHandler = UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<UnityEngine.Object>;

namespace com.keg.addressableloadmanagement
{
    public class AddressableManager
    {
#if UNITY_EDITOR
		[UnityEditor.MenuItem("KEG Addressables/Build Player Content")]
        public static void BuildAddressables()
        {
            UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
        }
#endif

        private static AddressableManager _instance;
        public static AddressableManager Get()
        {
            if( _instance == null )
            {
                _instance = new AddressableManager();
            }

            return _instance;
        }

        public enum AddressableInitSteps
        {
            Failed = -1,
            NotStarted = 0,
            Initializing = 1,
            Complete = 2
        }

        public enum GroupLoadSteps
        {
            Failed = -1,
            NotStarted = 0,
            Loading = 1,
            Complete = 2
        }

        private AddressableInitSteps _addressableInitStep = AddressableInitSteps.NotStarted;

        private Dictionary<string, ResourceManagementIResourceLocator> _pathToLocatorMap;
        private Dictionary<string, GroupLoadSteps> _groupToLoadStepMap;
		private Dictionary<string, (int, UnityEngine.Object)> _assetRefCount;
        private Dictionary<string, string> _assetNameToPathMap;
		private List<IAddressableLoader> _queuedLoaders;

        private AddressableManager()
        {
            _addressableInitStep = AddressableInitSteps.NotStarted;
            InitializeAddressables();
        }

        private bool InitializeAddressables()
        {
            if( _addressableInitStep != AddressableInitSteps.Complete )
            {
                if( _addressableInitStep == AddressableInitSteps.NotStarted )
                {
                    _addressableInitStep = AddressableInitSteps.Initializing;
                    var handle = Addressables.InitializeAsync();
                    handle.Completed += OnAddressablesInitialized;
                }

                return false;
            }

            return true;
        }

        private void OnAddressablesInitialized( AsyncOperationHandle<AddressableAssetsIResourceLocator> handle )
        {
            if( handle.IsDone && handle.Status == AsyncOperationStatus.Succeeded )
            {
                _addressableInitStep = AddressableInitSteps.Complete;
                ProcessQueue();
            }
            else
            {
                _addressableInitStep = AddressableInitSteps.Failed;
                Debug.LogErrorFormat( "Couldn't load through addressables becuase addressables couldn't initialize" );
            }
        }

        public void QueueLoader( IAddressableLoader loader )
        {
            if( _queuedLoaders == null )
            {
                _queuedLoaders = new List<IAddressableLoader>();
            }

            //increment ref count for this group--this is to aid in cleaning up a group
            IncrementAssetRefCount( loader.addressablePath );

            _queuedLoaders.Add( loader );
            ProcessQueue();
        }

		private void IncrementAssetRefCount( string assetPath )
		{
			if( _assetRefCount == null )
			{
				_assetRefCount = new Dictionary<string, (int, UnityEngine.Object)>();
			}
			if( !_assetRefCount.ContainsKey( assetPath ) )
			{
				_assetRefCount.Add( assetPath, (0, null) );
			}

            (int, UnityEngine.Object) tuple = _assetRefCount[ assetPath ];
            tuple.Item1 += 1;
            _assetRefCount[ assetPath ] = tuple;
		}

        private void ProcessQueue()
        {
            //initialize maps
            if( _pathToLocatorMap == null )
            {
                _pathToLocatorMap = new Dictionary<string, ResourceManagementIResourceLocator>();
            }
            if( _groupToLoadStepMap == null )
            {
                _groupToLoadStepMap = new Dictionary<string, GroupLoadSteps>();
            }

            //if we have nothing queued, don't do anything
            if( _queuedLoaders == null || _queuedLoaders.Count == 0 )
            {
                return;
            }
            //if we aren't done intiailizing, don't do anything
            if( _addressableInitStep != AddressableInitSteps.Complete )
            {
                return;
            }

            //iterate through queued loaders
            for( int i = _queuedLoaders.Count - 1; i >= 0; --i )
            {
                IAddressableLoader loader = _queuedLoaders[ i ];
                if( _pathToLocatorMap.ContainsKey( loader.addressablePath ) )
                {
                    //start load and remove from queue
                    Addressables.LoadAssetAsync<UnityEngine.Object>( _pathToLocatorMap[ loader.addressablePath ] ).Completed += ( AssetAsyncHandler handler ) =>
                    {
                        RecordAssetNameToPath( loader.addressablePath, handler.Result );
                        loader.OnLoadComplete( handler );
                    };
                    _queuedLoaders.RemoveAt( i );
                }
                else if( !_groupToLoadStepMap.ContainsKey( loader.addressableGroup ) )
                {
                    //start loading the group associated with queued loader
                    string group = loader.addressableGroup;

                    _groupToLoadStepMap.Add( group, GroupLoadSteps.Loading );

                    Addressables.LoadResourceLocationsAsync( loader.addressableGroup ).Completed += ( GroupAsyncHandler handle ) =>
                    {
                        OnResourceGroupLocationLoaded( group, handle );
                    };
                }
            }
        }

		private void RecordAssetNameToPath( string path, UnityEngine.Object asset )
		{
			if( _assetNameToPathMap == null )
			{
                _assetNameToPathMap = new Dictionary<string, string>();
			}
            _assetNameToPathMap[ asset.name ] = path;

            (int, UnityEngine.Object) tuple = _assetRefCount[ path ];
            tuple.Item2 = asset;
            _assetRefCount[ path ] = tuple;
		}

        private void OnResourceGroupLocationLoaded( string group, GroupAsyncHandler handle )
        {
            if( handle.IsDone && handle.Status == AsyncOperationStatus.Succeeded )
            {
                _groupToLoadStepMap[ group ] = GroupLoadSteps.Complete;

                var locators = handle.Result;
                int count = locators.Count;
                for( int i = 0; i < count; ++i )
                {
                    string path = locators[ i ].PrimaryKey;
                    _pathToLocatorMap.Add( path, locators[ i ] );
                }

				if( count == 0 )
				{
                    Debug.LogErrorFormat( "<color=red>ERROR:</color> locators not found for group <color=cyan>{0}</color>; cannot load anything for that group!", group );
				}

                ProcessQueue();
            }
            else
            {
                _groupToLoadStepMap[ group ] = GroupLoadSteps.Failed;
                Debug.LogErrorFormat( "Failed to load group {0}", group );
            }
        }

		public void FreeResource( UnityEngine.Object instantiatedAsset )
		{
            string name = instantiatedAsset.name;
            if( name.EndsWith( "(Clone)" ) )
            {
                name = name.Substring( 0, name.Length - 7 );
            }

            if( instantiatedAsset is UnityEngine.MonoBehaviour )
			{
                Debug.LogWarningFormat( "<color=yellow>WARNING:</color> Attempting to release monobehaviour attached to <color=cyan>{0}</color>--releasing associated game object instead.", name );
                instantiatedAsset = ( (UnityEngine.MonoBehaviour)instantiatedAsset ).gameObject;
			}
            if( instantiatedAsset is UnityEngine.GameObject )
            {
                UnityEngine.Object.Destroy( instantiatedAsset );
                Debug.LogFormat( "<color=green>INFO:</color> Destroyed instance of <color=cyan>{0}</color>.", name );
            }

            if( _assetRefCount == null )
			{
                Debug.LogError( "<color=red>ERROR:</color> No recorded assets to release." );
                return;
			}
			if( _assetNameToPathMap == null )
			{
                Debug.LogError( "<color=red>ERROR:</color> No recorded asset names to paths." );
                return;
			}

			if( !_assetNameToPathMap.ContainsKey( name ) )
			{
                Debug.LogErrorFormat( "<color=red>ERROR:</color> Couldn't trace asset <color=cyan>{0}</color> to a path.", name );
                return;
			}

            string path = _assetNameToPathMap[ name ];
			if( !_assetRefCount.ContainsKey( path ) )
			{
                Debug.LogErrorFormat( "<color=red>ERROR:</color> Couldn't find ref count for asset <color=cyan>{0}</color>.", name );
                return;
			}

            (int, UnityEngine.Object) refCount = _assetRefCount[ path ];
            refCount.Item1 = refCount.Item1 > 0 ? refCount.Item1 - 1 : 0;
            if( refCount.Item1 <= 0 )
            {
                Addressables.Release( refCount.Item2 );
                Debug.LogFormat( "<color=green>INFO:</color> 0 references to <color=cyan>{0}</color>--released asset.", name );
            }
            _assetRefCount[ path ] = refCount;
		}
    }
}
