# MineCombat - 我的世界卡牌战斗游戏

[![Unity Version](https://img.shields.io/badge/Unity-2022.3+-blue.svg)](https://unity3d.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen.svg)]()

## 🎮 项目介绍

**MineCombat** 是一款基于Unity开发的卡牌战斗游戏，灵感来源于《我的世界》(Minecraft)。这是宇宙超级无敌大(SuperBig42)团队参加网易雷火游戏制作大赛的作品。

### ✨ 核心特性

- 🃏 **丰富的卡牌系统** - 支持多种卡牌类型和稀有度
- ⚔️ **策略战斗** - 基于卡牌组合的深度策略玩法
- 🎯 **拖拽出牌** - 直观的拖拽交互系统
- 🏗️ **模块化架构** - 高度可扩展的代码结构
- 📊 **事件驱动** - 灵活的事件管理系统
- 🎨 **Minecraft风格** - 经典的像素艺术风格

## 🚀 快速开始

### 环境要求

- Unity 2022.3 或更高版本
- .NET Standard 2.1
- 支持C# 8.0+的IDE

### 安装步骤

1. **克隆仓库**
   ```bash
   git clone https://github.com/your-username/mine-kill.git
   cd mine-kill
   ```

2. **打开项目**
   - 使用Unity Hub打开项目文件夹
   - 等待Unity自动导入所有资源

3. **开始开发**
   - 打开 `Assets/Scenes/Game.unity` 场景
   - 点击Play按钮开始游戏

## 🏗️ 项目架构

### 目录结构

```
Assets/
├── _Scripts/                 # 核心脚本
│   ├── Base/                # 基础类
│   │   ├── Card.cs         # 卡牌基类
│   │   ├── Entity.cs       # 实体类
│   │   ├── Properties.cs   # 属性管理器
│   │   └── Damage.cs       # 伤害系统
│   ├── Controller/          # 控制器
│   │   ├── CardManager.cs  # 卡牌管理器
│   │   └── CombatManager.cs # 战斗管理器
│   ├── System/             # 系统组件
│   │   ├── CardSystem.cs   # 卡牌系统
│   │   └── CardDragSystem.cs # 拖拽系统
│   ├── Views/              # 视图组件
│   │   ├── CardView.cs     # 卡牌视图
│   │   └── HandView.cs     # 手牌视图
│   ├── Data/               # 数据类
│   │   ├── PlayArea.cs     # 出牌区域
│   │   └── CardSlot.cs     # 卡牌槽位
│   └── Tools/              # 工具类
│       ├── Parser.cs       # 字符串解析器
│       └── DragSystemSetup.cs # 拖拽系统设置
├── Scenes/                 # 游戏场景
├── Resources/              # 游戏资源
└── CardsIngredient/        # 卡牌素材
```

### 核心模块

#### 1. 卡牌系统 (Card System)
- **Card**: 卡牌基类，定义卡牌的基本属性
- **Material**: 材料类卡牌
- **CardSystem**: 卡牌游戏逻辑管理
- **CardDragSystem**: 拖拽出牌系统

#### 2. 实体系统 (Entity System)
- **Entity**: 游戏实体基类
- **Player**: 玩家类
- **Combatant**: 战斗者类

#### 3. 属性系统 (Properties System)
- **Properties**: 通用属性管理器
- 支持多种数据类型的高性能存储

#### 4. 事件系统 (Event System)
- **EventManager**: 事件管理器
- 支持优先级、分支和随机事件

#### 5. 伤害系统 (Damage System)
- **Damage**: 伤害类
- **DamageModifier**: 伤害修改器
- 支持多种伤害类型和修改机制

## 📚 API 文档

### 核心接口

#### Card 类
```csharp
public abstract class ACard : Properties, IEquatable<ACard>
{
    public readonly uint id;           // 卡牌ID
    public readonly Rarity rarity;     // 稀有度
    public readonly ITags tags;        // 标签
    public string Name { get; }        // 卡牌名称
    public string Description { get; } // 卡牌描述
}
```

#### Entity 类
```csharp
public class Entity : Properties
{
    public double GetHealth();         // 获取生命值
    public double GetMaxHealth();      // 获取最大生命值
    public void SetHealth(double health); // 设置生命值
    public bool ApplyDamage(double damage); // 应用伤害
    public bool IsAlive();             // 是否存活
}
```

#### Properties 类
```csharp
public class Properties : ICloneable<Properties>
{
    // 存储数据
    public bool Store(string name, int value);
    public bool Store(string name, double value);
    public bool Store(string name, bool value);
    public bool Store(string name, string value);
    public bool Store<T>(string name, T value) where T : notnull;
    
    // 更新数据
    public bool Update(string name, int value);
    public bool Update(string name, double value);
    public bool Update(string name, bool value);
    public bool Update(string name, string value);
    public bool Update<T>(string name, T value) where T : notnull;
    
    // 获取数据
    public int? GetInt(string name, bool checkDefault = true);
    public double? GetDouble(string name, bool checkDefault = true);
    public bool? GetBool(string name, bool checkDefault = true);
    public string? GetString(string name, bool checkDefault = true);
    public T? Get<T>(string name, bool checkDefault = true) where T : notnull;
    
    // 修改数据
    public bool Change(string name, Process<int> processor);
    public bool Change(string name, Process<double> processor);
    public bool Change(string name, Process<bool> processor);
    public bool Change(string name, Process<string> processor);
    public bool Change<T>(string name, Process<T> processor) where T : notnull;
}
```

#### EventManager 类
```csharp
public static class EventManager
{
    // 事件绑定
    public static void Bind(string eventName, Action action, int priority = 0);
    public static void Bind<T>(string eventName, Action<T> action, int priority = 0);
    
    // 事件触发
    public static void Trigger(string eventName);
    public static void Trigger<T>(string eventName, T parameter);
    
    // 分支事件
    public static void Bind(string eventName, string branch, Action action, int priority = 0);
    public static void Trigger(string eventName, string branch);
    
    // 随机事件
    public static void Bind(string eventName, string itemId, Action action, int priority = 0);
    public static void Trigger(string eventName);
}
```

#### Parser 类
```csharp
public static class Parser
{
    // 解析为集合
    public static IEnumerable<object>? ToCollection(string src, byte limit = 255, bool strict = false);
    
    // 解析为Box数组
    public static Box<string>?[]? ToBoxArray(string src, bool strict = false);
    
    // 解析为Box
    public static Box<string>? ToBox(string src, bool strict = false);
}
```

### 拖拽系统 API

#### CardDragSystem
```csharp
public class CardDragSystem : Singleton<CardDragSystem>
{
    public bool IsDragging { get; }           // 是否正在拖拽
    public CardView DraggedCard { get; }      // 当前拖拽的卡牌
    
    public void StartDrag(CardView cardView); // 开始拖拽
    public void EndDrag();                    // 结束拖拽
    public void CancelDrag();                 // 取消拖拽
}
```

#### PlayArea
```csharp
public class PlayArea : MonoBehaviour, IPlayArea
{
    public bool CanPlayCard(CardView cardView); // 是否可以出牌
    public bool TryPlayCard(CardView cardView); // 尝试出牌
    public void SetHighlight(bool highlight);   // 设置高亮
}
```

## 🎯 使用示例

### 创建卡牌
```csharp
// 创建基础卡牌
Card card = Card.Create("Diamond Sword", 3, false, Rarity.Rare, 
    new Tags(), Target.Selected, "damage:10");

// 创建材料卡牌
Material material = Material.Create("Iron Ingot", Rarity.Common, 
    new Tags());
```

### 事件系统使用
```csharp
// 绑定事件
EventManager.Bind("CardPlayed", (Entity player, Card card) => {
    Debug.Log($"{player.Name} 打出了 {card.Name}");
});

// 触发事件
EventManager.Trigger("CardPlayed", (player, card));
```

### 属性管理
```csharp
Entity player = new Player("Steve", 100.0);

// 存储属性
player.Store("attack", 10);
player.Store("defense", 5);

// 获取属性
int? attack = player.GetInt("attack");
int? defense = player.GetInt("defense");

// 修改属性
player.Change("attack", (ref int value) => value += 2);
```

### 拖拽系统设置
```csharp
// 自动设置拖拽系统
DragSystemSetup setup = gameObject.AddComponent<DragSystemSetup>();
setup.SetupDragSystem();

// 手动添加拖拽行为
CardView cardView = GetComponent<CardView>();
cardView.gameObject.AddComponent<CardDragBehavior>();
```

## 🛠️ 开发指南

### 添加新卡牌
1. 在 `Assets/Resources/Cards/` 添加卡牌图片
2. 在 `CardDatabaseSystem` 中注册卡牌
3. 使用 `Card.Create()` 创建卡牌实例

### 添加新事件
1. 在 `EventManager` 静态构造函数中添加事件
2. 使用 `EventManager.Bind()` 绑定处理函数
3. 使用 `EventManager.Trigger()` 触发事件

### 自定义出牌区域
```csharp
public class CustomPlayArea : PlayArea
{
    protected override void OnCardPlayed(CardView cardView)
    {
        base.OnCardPlayed(cardView);
        // 自定义出牌逻辑
    }
}
```

## 🐛 故障排除

### 常见问题

1. **卡牌无法拖拽**
   - 检查 `CardDragBehavior` 组件是否存在
   - 确认 `EventSystem` 存在且正常工作

2. **事件不触发**
   - 检查事件名称是否正确
   - 确认事件已正确绑定

3. **属性获取失败**
   - 检查属性名称是否正确
   - 确认属性类型匹配

## 🤝 贡献指南

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 👥 团队

- **宇宙超级无敌大(SuperBig42)** - 开发团队

## 🙏 致谢

- Unity Technologies - 游戏引擎
- Minecraft - 灵感来源
- 网易雷火 - 比赛平台

---

**注意**: 本项目仍在积极开发中，API可能会发生变化。请关注更新日志。

