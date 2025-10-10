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
1.  使用静态类EventManager管理事件，用户只能调用唯一的外部接口Bind来绑定新的Action
```cs
EventManager.Bind("SomeEvent", yourAction)
```
2.  事件在EventManager的static块中硬编码注册，后续也许会改成读文件注册
3.  在程序内部可以任意调用EventManager.Trigger<T>(name, <T>paras)来执行绑定的所有Action，参数按值传递
4.  若传递多个参数，将其以元组形式传递 --> (string a, int b, bool c)
5.  绑定的函数可以接收元组作为参数，也可以直接接受多个参数，但必须无返回值，支持无参数函数

#### Parser的使用说明
1.  Parser是一个静态工具类，用于处理字符串到任意集合的转换，一般返回一个List或HashSet
2.  目前只有ToCollection一个方法，返回无字典集合，第二个参数控制了最大深度（如果存在第x+1层结构，直接返回null或报错），第三个参数控制是否严格测试（默认false，若为true，任何异常都会报错，否则返回null）
```cs
Parser.ToCollection("{a, b, {{c, d}, e}}", 3, false) //返回该结构的HashSet<object>
Parser.ToCollection("{a, b, {{c, d}, e}}", 2, false) //返回null
Parser.ToCollection("{a, b, {{c, d}, e}}", 2, true) //报错
```

#### 属性管理器
1.  Properties是经过优化的属性管理器，支持int、double、bool三个基础类型和所有引用类型，且对string有优化
2.  Properties对象的使用方式非常简单，提供Store函数用于存入数据，Update函数用于更新数据，Change函数用于修改数据，Get的一系列衍生函数用于获取数据
```cs
/*下面所有“id”均为一个字符串，用于标识数据
 *不同类型的数据使用同一个id可能会导致冲突，避免这样做*/

//自动识别类型，无需泛型声明
properties.Store(id, 36.5); //若id已存在，返回假，不操作
properties.Update(id, "OftenOviour"); //若id不存在，返回假，但仍会创建属性并赋值
properties.Change(id, (ref Damage dmg) => { /*对dmg的一些操作*/ }); //若id不存在，返回假，不操作

//需要显示指定要获取的类型
properties.Get<Damage>(id)； //返回值为Damage或null，性能稍差，不可以使用Get<int>、Get<double>、Get<bool>、Get<string>，必然会导致返回值错误（不一定为null）
properties.GetInt(id)； //返回值为int或null，需要空判断，double、bool、string都有对应的方法，它们性能更好
```

#### 标签和标签集合
1.  一般来说不必关注ITag，只需关注ITags，它有两个实现类StaticTags和Tags。前者是内存可空且不可修改的，只能包含一级结构，初始化性能更好；后者必定占用内存，可以动态修改，可以包含二级结构
2.  Match行为意味着发起者将遍历自己的一级结构，只要其中任何一项（如果是二级结构，则为该项的每个内容）可以在目标中找到，则视为成功
3.  TagsManager是存储常用标签集合的一个选择，Tags对象的构建开销较大，让常用的Tags指向同一个引用是不错的选择

#### 伤害系统
1.  DamageTypes管理所有伤害类型对应的标签集合，Ignore方法用于测试指定的伤害类型是否被提供标签集合的对象忽略
2.  DamageModifier是滞后的伤害修改机制，内含有值，优先级和一个标签集合，可以通过工厂方法生产ADD型，MUL型，MULTOTAL型或CUSTOM型，其中CUSTOM型需要传入一个处理double函数
3.  事件"DamageProcess"是即时的伤害修改机制，直接接触Damage对象并修改伤害，需要手动提供一个标签集合并Ignore，否则总是生效；也可以在该事件中添加DamageModifier

#### 参与贡献
1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request

