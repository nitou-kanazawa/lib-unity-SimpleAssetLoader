using System;
using Cysharp.Threading.Tasks;

namespace Nitou.AssetLoader
{
    /// <summary>
    ///     <see cref="AssetLoadHandle{T}"/> の内部状態を設定するためのインターフェース．
    ///     ローダー実装からのみ使用される実装詳細のため，アセンブリ外には公開しない．
    /// </summary>
    internal interface IAssetLoadHandleSetter<T>
    {
        void SetStatus(AssetLoadStatus status);

        void SetResult(T result);

        void SetPercentCompleteFunc(Func<float> percentComplete);

        void SetTask(UniTask<T> task);

        void SetOperationException(Exception ex);
    }
}
