# RabbitMQ.Demos
包括两个Demo：
- 一个简单的WorkQueue代码演示。
- 一个多交换机代码演示。

## Screenshot:

![RabbitMQ.MixMultiExchange](https://raw.githubusercontent.com/yongfa365/RabbitMQ.Demos/master/RabbitMQ.MixMultiExchange.png)

# 多交换机介绍：

所有请求来后都先经过第一层交换机，此为fanout交换机，它会将消息给到后面的每一个交换机。

后面有两个交换机：

一个只用来写日志，默认不与队列绑定，有消息直接抛弃，有需要时在界面UI上绑定，很灵活。

另一个交换机，只处理业务逻辑，不用记录日志。
