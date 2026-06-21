#if USE_ADDRESSABLES
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Nitou.AssetLoader
{
    /// <summary>
    ///     Addressablesを用いたアセット読み込みの実装．
    ///     同期読み込み（<see cref="Load{T}"/>）はAddressables 1.17.4以降でのみ利用可能．
    /// </summary>
    public sealed class AddressableAssetLoader : IAssetLoader
    {
        private readonly Dictionary<int, AsyncOperationHandle> _controlIdToHandles = new();

        private int _nextControlId;

        /// <inheritdoc/>
        public AssetLoadHandle<T> Load<T>(string key) where T : Object
        {
#if ADDRESSABLES_1_17_4_OR_NEWER
            var addressableHandle = Addressables.LoadAssetAsync<T>(key);
            addressableHandle.WaitForCompletion();
            var controlId = _nextControlId++;
            _controlIdToHandles.Add(controlId, addressableHandle);

            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            setter.SetPercentCompleteFunc(() => addressableHandle.PercentComplete);
            setter.SetTask(UniTask.FromResult(addressableHandle.Result));
            setter.SetResult(addressableHandle.Result);
            var status = addressableHandle.Status == AsyncOperationStatus.Succeeded
                ? AssetLoadStatus.Success
                : AssetLoadStatus.Failed;
            setter.SetStatus(status);
            setter.SetOperationException(addressableHandle.OperationException);
            return handle;
#else
            throw new NotSupportedException();
#endif
        }

        /// <inheritdoc/>
        public AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object
        {
            var addressableHandle = Addressables.LoadAssetAsync<T>(key);
            var controlId = _nextControlId++;
            _controlIdToHandles.Add(controlId, addressableHandle);

            var handle = new AssetLoadHandle<T>(controlId);
            var setter = (IAssetLoadHandleSetter<T>)handle;
            var tcs = new UniTaskCompletionSource<T>();
            addressableHandle.Completed += x =>
            {
                setter.SetResult(x.Result);
                var status = x.Status == AsyncOperationStatus.Succeeded
                    ? AssetLoadStatus.Success
                    : AssetLoadStatus.Failed;
                setter.SetStatus(status);
                setter.SetOperationException(addressableHandle.OperationException);
                tcs.TrySetResult(x.Result);
            };

            setter.SetPercentCompleteFunc(() => addressableHandle.PercentComplete);
            setter.SetTask(tcs.Task);
            return handle;
        }

        /// <inheritdoc/>
        public void Release(AssetLoadHandle handle)
        {
            if (!_controlIdToHandles.Remove(handle.ControlId, out var addressableHandle))
                throw new InvalidOperationException($"There is no asset that has been requested for release (ControlId: {handle.ControlId}).");

            Addressables.Release(addressableHandle);
        }
    }
}
#endif
