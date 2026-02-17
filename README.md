# monitor-blackout

`monitor-blackout` は、特定モニタを黒いオーバーレイで覆う Windows 向けユーティリティです。  
特に、デュアルモニタやマルチモニタ環境で「複数モニタのうち 1 枚を丸ごと黒くして視界を整理したい」人を主な対象としています。

## 目的
映画視聴や集中作業時に、サブモニタの視界ノイズを消すために使います。

## この方式を採用した理由
モニタ無効化（`Win+P` や物理電源 OFF）は、環境によってウィンドウ再配置が発生することがあります。  
`monitor-blackout` はディスプレイ構成を変えずに黒幕だけを重ねる方式を採用し、作業中のウィンドウ配置を崩しにくくしています。

## 解除方法
黒い画面を 1 回クリックすると解除されます（詰み防止）。

## 動作仕様（初期実装）
- `MonitorBlackout.Toggle`: アクティブウィンドウが存在するモニタを判定し、ON/OFF をトグルして終了
- `MonitorBlackout.Overlay`: 対象モニタを黒い全画面ウィンドウで覆い、クリックまたは停止シグナルで終了
- 同一モニタ上での多重起動は `Mutex` で防止

## 使い方
1. 黒幕を出したいモニタ上の任意ウィンドウをアクティブにする
2. `MonitorBlackout.Toggle.exe` を実行する（ON）
3. 再実行するか、黒幕を 1 クリックすると解除される（OFF）

## ビルド
```powershell
dotnet build MonitorBlackout.slnx
```

## self-contained 配布例
```powershell
dotnet publish src/MonitorBlackout.Toggle/MonitorBlackout.Toggle.csproj -c Release -r win-x64 --self-contained true -o publish
dotnet publish src/MonitorBlackout.Overlay/MonitorBlackout.Overlay.csproj -c Release -r win-x64 --self-contained true -o publish
```
