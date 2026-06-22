# Simple Asset Loader

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

## 概要
Unityのリソース読み込みの仕組みである `Resources`と`Addressable` 統一的に扱うシンプルなライブラリ．

## 特徴

- **統一インターフェース**: Resources と Addressable の違いを隠蔽し、同じ API で扱えます
- **同期・非同期読み込み**: 両方の読み込み方式をサポート
- **進捗監視**: 非同期読み込み時の進捗状況を取得可能
- **エラーハンドリング**: 読み込み失敗時の例外情報を取得可能

> [!caution]
> キャッシュ、プリロード、リトライなどの機能は、サポートしません。

## セットアップ

### 要件 / 開発環境
- Unity 6000.0
- UniTask (Cysharp.Threading.Tasks)

### インストール

1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下のURLを入力する
```
https://github.com/nitou-kanazawa/lib-unity-SimpleAssetLoader.git
```

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記
```
{
    "dependencies": {
        "jp.nitou.simple-assetloader": "https://github.com/nitou-kanazawa/lib-unity-SimpleAssetLoader.git"
    }
}
```

## 使用方法

### 基本的な使用例

```csharp
using Nitou.AssetLoader;

// Addressableを使用する場合
IAssetLoader loader = new AddressableAssetLoader();

// Resourcesを使用する場合
// IAssetLoader loader = new ResourcesAssetLoader();

// 非同期読み込み
var handle = loader.LoadAsync<Texture2D>("my_texture");
await handle.Task; // 完了を待つ

if (handle.Status is AssetLoadStatus.Success)
{
    var texture = handle.Result;
    // テクスチャを使用
}

// リソース解放
loader.Release(handle);
```

### ScriptableObjectを使用する場合

```csharp
// Inspectorで設定したAddressableAssetLoaderSOを使用
[SerializeField] private AddressableAssetLoaderSO _assetLoader;

private async void LoadAsset()
{
    var handle = _assetLoader.LoadAsync<Sprite>("my_sprite");
    await handle.Task;
    
    if (handle.Status is AssetLoadStatus.Success)
    {
        // スプライトを使用
        image.sprite = handle.Result;
    }
    
    _assetLoader.Release(handle);
}
```

### 進捗監視

```csharp
var handle = loader.LoadAsync<AudioClip>("my_sound");

// 進捗を監視
while (!handle.IsDone)
{
    float progress = handle.PercentComplete;
    Debug.Log($"読み込み進捗: {progress * 100:F1}%");
    await UniTask.Yield();
}
```

## アーキテクチャ設計

### Strategy パターン

```cs
public interface IAssetLoader
{
    AssetLoadHandle<T> Load<T>(string key) where T : Object;
    AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object;
    void Release(AssetLoadHandle handle);
}
```
- ResourcesとAddressablesをIAssetLoaderインターフェースで抽象化
  
### Handle パターン
- AssetLoadHandleによる統一的な操作管理
- ControlIdによる追跡可能性
- ステータス管理の明確化


## 注意事項

- Addressableを使用する場合は、プロジェクトにAddressablesパッケージがインストールされている必要があります
- Resourcesを使用する場合は、アセットがResourcesフォルダ内に配置されている必要があります
- 読み込みしたアセットは必ず`Release`メソッドで解放してください
- 非同期読み込み時は`await handle.Task`で完了を待つことを推奨します

