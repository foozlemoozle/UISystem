using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AddressableAssetsIResourceLocator = UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator;
using ResourceManagementIResourceLocator = UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation;
using GroupAsyncHandler = UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation>>;

namespace com.keg.uisystem
{
    public class AddressableManager
    {
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
        private List<IAddressableUILoader> _queuedLoaders;

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

        public void QueueLoader( IAddressableUILoader loader )
        {
            if( _queuedLoaders == null )
            {
                _queuedLoaders = new List<IAddressableUILoader>();
            }

            _queuedLoaders.Add( loader );
            ProcessQueue();
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
                IAddressableUILoader loader = _queuedLoaders[ i ];
                if( _pathToLocatorMap.ContainsKey( loader.addressablePath ) )
                {
                    //start load and remove from queue
                    Addressables.InstantiateAsync( _pathToLocatorMap[ loader.addressablePath ] ).Completed += loader.OnLoadComplete;
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

                ProcessQueue();
            }
            else
            {
                _groupToLoadStepMap[ group ] = GroupLoadSteps.Failed;
                Debug.LogErrorFormat( "Failed to load group {0}", group );
            }
        }
    }
}
