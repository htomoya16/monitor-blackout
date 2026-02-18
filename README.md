# monitor-blackout

`monitor-blackout` は、指定した 1 枚のモニタを黒い全画面オーバーレイで覆う Windows 向けユーティリティです。  
映画やアニメを集中して見たいとき、サブモニタに出る光・通知・開きっぱなしのウィンドウを一時的に隠したい場面を想定しています。

このツールは「黒くしたい画面で `MonitorBlackout.Toggle.exe` を起動するだけ」で ON/OFF を切り替えられます。  
`Win+P` や物理電源 OFF のようにディスプレイ構成を変えずに使えるため、ウィンドウ再配置の手間を減らせるのが特徴です。

## デモ動画
後で動画を追加予定です。

## 使い方
普段実行するのは `MonitorBlackout.Toggle.exe` だけです。  
`MonitorBlackout.Overlay.exe` は `Toggle` から自動起動されるため、通常は直接実行しません。

1. 黒くしたい（オフにしたい）画面上で `MonitorBlackout.Toggle.exe` を起動する（ON）
2. 解除するときは次のどちらか
   - `MonitorBlackout.Toggle.exe` をもう一度実行する（OFF）
   - 黒幕を 1 回クリックする（OFF）

ショートカットを作成して、よく黒幕化したいモニタ側に置いておく運用をおすすめします。

## 既知の制約
- Exclusive Fullscreen（DirectX など）では最前面表示が制限される場合があります
- 環境によってはセキュリティソフトが初回実行時に検査を行う場合があります

## 開発/配布
```powershell
dotnet build MonitorBlackout.slnx
dotnet publish src/MonitorBlackout.Toggle/MonitorBlackout.Toggle.csproj -c Release -r win-x64 --self-contained true -o publish
dotnet publish src/MonitorBlackout.Overlay/MonitorBlackout.Overlay.csproj -c Release -r win-x64 --self-contained true -o publish
```

`publish` フォルダに出力された 2 つの exe を同じフォルダのまま Zip 化して配布します。
