using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.keg.uisystem
{
    public class UIReferenceHolder : MonoBehaviour
    {
        private static UIReferenceHolder _get;
        public static UIReferenceHolder Get => _get;

        [SerializeField]
        [RequiredField]
        private DeviceFrame _layerPrefab;
        public DeviceFrame layerPrefab => _layerPrefab;

		public void Awake()
		{
            _get = this;
		}
	}
}
