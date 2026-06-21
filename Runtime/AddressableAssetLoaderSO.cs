#if USE_ADDRESSABLES
using UnityEngine;

namespace Nitou.AssetLoader
{
    [CreateAssetMenu(
        fileName = "AddressableAssetLoader",
        menuName = "Resource Loader/Addressable Asset Loader")
    ]
    public sealed class AddressableAssetLoaderSO : AssetLoaderSO
    {
        private readonly AddressableAssetLoader _loader = new();

        /// <inheritdoc/>
        public override AssetLoadHandle<T> Load<T>(string key)
        {
            return _loader.Load<T>(key);
        }

        /// <inheritdoc/>
        public override AssetLoadHandle<T> LoadAsync<T>(string key)
        {
            return _loader.LoadAsync<T>(key);
        }

        /// <inheritdoc/>
        public override void Release(AssetLoadHandle handle)
        {
            _loader.Release(handle);
        }
    }
}
#endif
