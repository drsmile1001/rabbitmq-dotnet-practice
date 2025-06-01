# 🐇 RabbitMQ 基礎練習 (.NET 6)

本專案示範如何使用 **RabbitMQ** 與 **.NET 6** 建立最基本的 **Pub/Sub** 與 **Work Queues** 模型，目的是理解訊息發佈與消費的解耦機制與佇列分配行為。

## ✨ 實作概念

本專案分為兩個角色：

- **Sender（發佈端）**：持續發送訊息至指定 queue。
- **Receiver（消費端）**：訂閱 queue 並處理訊息。可啟動多個 consumer 實現負載分擔。

```
publisher -> exchanger -> queue_A -> consumerA1
                                  -> consumerA2
                       -> queue_B -> consumerB1
                                  -> consumerB2
```

- 若多個 consumer 綁定相同 queue，會**輪流消費**（負載平衡）
- 若綁定不同 queue，則會**分別獲得相同訊息**（訊息共享）

## 🔧 技術細節

- 使用 [RabbitMQ.Client](https://www.nuget.org/packages/RabbitMQ.Client/) NuGet 套件
- 使用 `BackgroundService` 建立持續運行的 sender/receiver
- `appsettings.json` 讀取連線設定
- queue 可透過 `QueueDeclare` 設定是否持久化、獨佔、刪除條件

## 📁 關鍵檔案

| 檔案名稱   | 說明                   |
|------------|------------------------|
| `Sender.cs`   | 建立 queue 並每秒送出訊息         |
| `Receiver.cs` | 綁定 queue 並列印接收到的訊息     |

## 🛠️ 執行方式

1. 建立並啟動本機 RabbitMQ（例如使用 Docker）
2. 設定 `appsettings.json` 連線資訊
3. 啟動 `Sender` 專案發送訊息
4. 可同時啟動多個 `Receiver`，觀察輪詢消費行為
