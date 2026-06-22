using System;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Nitou.AssetLoader
{
    public abstract class AssetLoadHandle
    {
        protected Func<float> _percentCompleteFunc;

        public int ControlId { get; }

        public AssetLoadStatus Status { get; protected set; }

        public Exception OperationException { get; protected set; }

        /// <summary>
        ///     読み込みの進捗（0.0〜1.0）．
        /// </summary>
        public float PercentComplete => _percentCompleteFunc?.Invoke() ?? 0f;

        public bool IsDone => Status is not AssetLoadStatus.None;

        protected AssetLoadHandle(int controlId)
        {
            ControlId = controlId;
        }
    }


    public sealed class AssetLoadHandle<T> : AssetLoadHandle, IAssetLoadHandleSetter<T>
        where T : Object
    {
        public T Result { get; private set; }

        public UniTask<T> Task { get; private set; }

        public AssetLoadHandle(int controlId) : base(controlId) { }

        void IAssetLoadHandleSetter<T>.SetStatus(AssetLoadStatus status)
        {
            Status = status;
        }

        void IAssetLoadHandleSetter<T>.SetResult(T result)
        {
            Result = result;
        }

        void IAssetLoadHandleSetter<T>.SetTask(UniTask<T> task)
        {
            Task = task;
        }

        void IAssetLoadHandleSetter<T>.SetOperationException(Exception ex)
        {
            OperationException = ex;
        }

        void IAssetLoadHandleSetter<T>.SetPercentCompleteFunc(Func<float> percentComplete)
        {
            _percentCompleteFunc = percentComplete;
        }
    }
}
