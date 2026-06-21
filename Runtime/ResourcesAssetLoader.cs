using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nitou.AssetLoader
{
    /// <summary>
    ///     <c>Resources</c> を用いたアセット読み込みの実装．
    /// </summary>
    public sealed class ResourcesAssetLoader : IAssetLoader
    {
        private int _nextControlId;

        /// <inheritdoc/>
        public AssetLoadHandle<T> Load<T>(string key) where T : Object
        {
            var controlId = _nextControlId++;

            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            var result = Resources.Load<T>(key);

            setter.SetResult(result);
            var status = (result != null) ? AssetLoadStatus.Success : AssetLoadStatus.Failed;
            setter.SetStatus(status);
            if (status is AssetLoadStatus.Failed)
            {
                var exception = new InvalidOperationException($"Requested asset（Key: {key}）was not found.");
                setter.SetOperationException(exception);
            }

            setter.SetPercentCompleteFunc(() => 1.0f);
            setter.SetTask(UniTask.FromResult(result));
            return handle;
        }

        /// <inheritdoc/>
        public AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object
        {
            var controlId = _nextControlId++;

            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            var tcs = new UniTaskCompletionSource<T>();

            var req = Resources.LoadAsync<T>(key);

            req.completed += _ =>
            {
                var result = req.asset as T;
                setter.SetResult(result);
                var status = (result != null) ? AssetLoadStatus.Success : AssetLoadStatus.Failed;
                setter.SetStatus(status);
                if (status is AssetLoadStatus.Failed)
                {
                    var exception = new InvalidOperationException($"Requested asset（Key: {key}）was not found.");
                    setter.SetOperationException(exception);
                }

                tcs.TrySetResult(result);
            };

            setter.SetPercentCompleteFunc(() => req.progress);
            setter.SetTask(tcs.Task);
            return handle;
        }

        /// <inheritdoc/>
        public void Release(AssetLoadHandle handle)
        {
            // Resources.UnloadUnusedAssets() is responsible for releasing assets loaded by Resources.Load(), so nothing is done here.
        }
    }
}
