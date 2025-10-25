# MineCombat 系统架构文档

## 系统概览

MineCombat 采用分层架构设计，主要分为以下几个层次：

```
┌─────────────────────────────────────────────────────────────┐
│                    表现层 (Presentation Layer)                │
├─────────────────────────────────────────────────────────────┤
│  CardView, HandView, CombatantView, PlayArea, CardDragBehavior │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                    控制层 (Controller Layer)                  │
├─────────────────────────────────────────────────────────────┤
│  CardManager, CombatManager, EventManager, CardSystem        │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                    业务层 (Business Layer)                   │
├─────────────────────────────────────────────────────────────┤
│  Card, Entity, Player, Combatant, Damage, Properties        │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                    数据层 (Data Layer)                       │
├─────────────────────────────────────────────────────────────┤
│  CardData, CardDatabase, Tags, Modifiers                    │
└─────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────┐
│                    工具层 (Utility Layer)                    │
├─────────────────────────────────────────────────────────────┤
│  Parser, Singleton, Translator, Randomizer                  │
└─────────────────────────────────────────────────────────────┘
```

## 核心模块详解

### 1. 卡牌系统 (Card System)

#### 类层次结构
```
ACard (抽象基类)
├── Card (具体卡牌)
└── Material (材料卡牌)
```

#### 核心组件
- **ACard**: 卡牌基类，定义基本属性和行为
- **Card**: 可使用的卡牌，包含费用、目标、命令等
- **Material**: 材料类卡牌，用于合成等
- **CardSystem**: 卡牌游戏逻辑管理
- **CardManager**: 卡牌模板管理

#### 数据流
```
CardData → Card → CardView → CardDragBehavior → PlayArea
```

### 2. 实体系统 (Entity System)

#### 类层次结构
```
Properties (属性基类)
└── Entity (实体基类)
    ├── Player (玩家)
    └── Combatant (战斗者)
```

#### 核心组件
- **Properties**: 通用属性管理器，支持多种数据类型
- **Entity**: 游戏实体基类，包含生命值管理
- **Player**: 玩家类，包含背包、状况栏等
- **Combatant**: 战斗者类，用于AI或NPC

#### 属性管理
```csharp
// 属性存储
entity.Store("attack", 10);
entity.Store("defense", 5);

// 属性获取
int? attack = entity.GetInt("attack");

// 属性修改
entity.Change("attack", (ref int value) => value += 2);
```

### 3. 事件系统 (Event System)

#### 事件类型
- **PriorityEvent**: 优先级事件，按优先级执行
- **SlicedEvent**: 分支事件，支持多个分支
- **RandomEvent**: 随机事件，按权重随机选择

#### 事件流程
```
事件注册 → 事件绑定 → 事件触发 → 事件执行
```

#### 使用示例
```csharp
// 注册事件
EventManager.Events.Add("CardPlayed", new PriorityEvent<Card>());

// 绑定事件
EventManager.Events["CardPlayed"].Bind((Card card) => {
    Debug.Log($"卡牌被使用: {card.Name}");
});

// 触发事件
EventManager.Events["CardPlayed"].Trigger(card);
```

### 4. 伤害系统 (Damage System)

#### 核心组件
- **Damage**: 伤害类，包含类型和数值
- **DamageModifier**: 伤害修改器基类
- **DamageModifierAdd**: 加法修改器
- **DamageModifierMul**: 乘法修改器
- **DamageModifierMulTotal**: 总乘法修改器
- **DamageModifierCustom**: 自定义修改器

#### 伤害计算流程
```
基础伤害 → 应用修改器 → 最终伤害
```

#### 修改器类型
```csharp
// 加法修改器
var addModifier = DamageModifiers.CreateAdd(10, 1, tags);

// 乘法修改器
var mulModifier = DamageModifiers.CreateMul(1.5, 2, tags);

// 自定义修改器
var customModifier = DamageModifiers.CreateCustom(
    (ref double damage) => damage *= 2, 3, tags);
```

### 5. 拖拽系统 (Drag System)

#### 组件关系
```
CardDragSystem (单例)
├── CardDragBehavior (拖拽行为)
├── PlayArea (出牌区域)
└── IPlayArea (出牌区域接口)
```

#### 拖拽流程
```
开始拖拽 → 更新位置 → 检测区域 → 结束拖拽 → 验证出牌
```

#### 核心接口
```csharp
public interface IPlayArea
{
    bool CanPlayCard(CardView cardView);
    bool TryPlayCard(CardView cardView);
    void SetHighlight(bool highlight);
}
```

### 6. 属性系统 (Properties System)

#### 数据类型支持
- **基础类型**: int, double, bool, string
- **引用类型**: 任意引用类型
- **性能优化**: 对string类型有特殊优化

#### 操作类型
- **Store**: 存储新属性（如果已存在则失败）
- **Update**: 更新属性（如果不存在则创建）
- **Change**: 修改属性（如果不存在则失败）
- **Get**: 获取属性值

#### 线程安全
- 使用锁机制保证线程安全
- 支持并发读写操作

## 设计模式

### 1. 单例模式 (Singleton)
```csharp
public class CardDragSystem : Singleton<CardDragSystem>
{
    // 单例实现
}
```

### 2. 工厂模式 (Factory)
```csharp
public static class DamageModifiers
{
    public static DamageModifierAdd CreateAdd(double value, int priority, ITags tags);
    public static DamageModifierMul CreateMul(double value, int priority, ITags tags);
    // 其他工厂方法
}
```

### 3. 观察者模式 (Observer)
```csharp
// 事件系统实现观察者模式
EventManager.Events["CardPlayed"].Bind(OnCardPlayed);
```

### 4. 策略模式 (Strategy)
```csharp
// 伤害修改器实现策略模式
public abstract class DamageModifier
{
    public abstract void Process(Damage damage);
}
```

### 5. 模板方法模式 (Template Method)
```csharp
public abstract class ACard
{
    // 定义卡牌的基本结构
    public abstract Card Clone();
}
```

## 数据流

### 卡牌出牌流程
```
1. 玩家拖拽卡牌
   ↓
2. CardDragBehavior 检测拖拽
   ↓
3. CardDragSystem 管理拖拽状态
   ↓
4. PlayArea 检测是否可以出牌
   ↓
5. CardSystem 验证费用和规则
   ↓
6. 执行卡牌效果
   ↓
7. 触发相关事件
```

### 伤害计算流程
```
1. 创建 Damage 对象
   ↓
2. 添加 DamageModifier
   ↓
3. 按优先级排序修改器
   ↓
4. 依次应用修改器
   ↓
5. 返回最终伤害值
```

### 事件处理流程
```
1. 注册事件类型
   ↓
2. 绑定事件处理函数
   ↓
3. 触发事件
   ↓
4. 按优先级执行处理函数
   ↓
5. 完成事件处理
```

## 扩展性设计

### 1. 卡牌扩展
- 继承 ACard 基类
- 实现特定的卡牌逻辑
- 注册到 CardManager

### 2. 事件扩展
- 在 EventManager 中注册新事件
- 绑定处理函数
- 在适当位置触发事件

### 3. 伤害修改器扩展
- 继承 DamageModifier 基类
- 实现 Process 方法
- 使用工厂方法创建

### 4. 出牌区域扩展
- 实现 IPlayArea 接口
- 定义特定的出牌规则
- 注册到 CardDragSystem

## 性能优化

### 1. 属性系统优化
- 使用 Lazy<T> 延迟初始化
- 分离不同数据类型的存储
- 使用锁机制保证线程安全

### 2. 事件系统优化
- 使用字典存储事件映射
- 按优先级排序执行
- 支持事件常量化

### 3. 拖拽系统优化
- 单例模式减少实例化
- 事件驱动减少轮询
- 对象池管理卡牌视图

## 测试策略

### 1. 单元测试
- 测试各个类的核心方法
- 验证边界条件
- 确保线程安全

### 2. 集成测试
- 测试模块间交互
- 验证数据流正确性
- 测试异常情况处理

### 3. 性能测试
- 测试大量卡牌的性能
- 验证内存使用情况
- 测试并发访问性能

## 部署和维护

### 1. 版本控制
- 使用语义化版本号
- 维护变更日志
- 标记重要版本

### 2. 文档维护
- 保持API文档更新
- 提供使用示例
- 记录设计决策

### 3. 代码质量
- 遵循编码规范
- 使用静态分析工具
- 定期重构代码

---

**注意**: 此架构文档会随着系统发展持续更新，请关注最新版本。
