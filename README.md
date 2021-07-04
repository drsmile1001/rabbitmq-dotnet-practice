# RabbitMQ 練習

本專案實踐了 pub/sub 和 Work queues 概念。發佈端只考慮發佈，不考慮誰來處理，達到解耦。
接受端聲明自己要取用什麼主題（這裡用exchanger代替），是哪種類型的處理器。而同類
處理器會依序消耗傳遞的訊息，達到負載平衡；異類的處理器有分別的佇列，達到訊息共享。

```
publisher -> exchanger -> queue_A -> consumerA1
                                  -> consumerA2
                       -> queue_B -> consumerB1
                                  -> consumerB2
```
