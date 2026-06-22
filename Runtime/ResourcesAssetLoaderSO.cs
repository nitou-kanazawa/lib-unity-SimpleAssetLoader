using UnityEngine;

namespace Nitou.AssetLoader
{
    [CreateAssetMenu(
        fileName = "ResourcesAssetLoader",
        menuName = "Resource Loader/Resources Asset Loader"
    )]
    public sealed class ResourcesAssetLoaderSO : AssetLoaderSO
    {
        private readonly ResourcesAssetLoader _loader = new();

        /// <inheritdoc/>
        public override AssetLoadHandle<T> Load<T>(string key) => _loader.Load<T>(key);

        /// <inheritdoc/>
        public override AssetLoadHandle<T> LoadAsync<T>(string key) => _loader.LoadAsync<T>(key);

        /// <inheritdoc/>
        public override void Release(AssetLoadHandle handle) => _loader.Release(handle);
    }
}
