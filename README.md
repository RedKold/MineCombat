# MineCombat

#### 介绍
We are 宇宙超级无敌大(SuperBig42) team who are taking part in the Netease Leihuo game-making competition. We are making a card combat game inspired by Minecraft.

#### 软件架构
基于Unity


#### 安装教程

1.  克隆此仓库
2.  使用Unity打开本仓库文件夹
3.  等待Unity自动根据package.json信息加载所需库
4.  开始游玩和开发。

#### 事件注册教程
1.  使用静态类EventManager管理事件，用户只能调用唯一的外部接口Bind来绑定新的Action --> EventManager.Bind("SomeEvent", yourAction)
2.  事件在EventManager的static块中硬编码注册，后续也许会改成读文件注册
3.  在程序内部可以任意调用EventManager.Trigger<T>(name, <T>paras)来执行绑定的所有Action，参数按值传递
4.  若传递多个参数，将其以元组形式传递 --> (string a, int b, bool c)
5.  绑定的函数可以接收元组作为参数，也可以直接接受多个参数，但必须无返回值，暂时不支持无参数函数

#### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request

