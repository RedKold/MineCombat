# MineCombat API 文档

## 目录
- [核心类](#核心类)
- [卡牌系统](#卡牌系统)
- [实体系统](#实体系统)
- [属性系统](#属性系统)
- [事件系统](#事件系统)
- [伤害系统](#伤害系统)
- [拖拽系统](#拖拽系统)
- [工具类](#工具类)

## 核心类

### ACard (抽象卡牌基类)

```csharp
public abstract class ACard : Properties, IEquatable<ACard>
```

**属性:**
- `uint id` - 卡牌唯一标识符
- `Rarity rarity` - 卡牌稀有度
- `ITags tags` - 卡牌标签集合
- `string Name` - 卡牌名称 (只读)
- `string Description` - 卡牌描述 (只读)

**方法:**
- `bool Equals(ACard other)` - 比较两个卡牌是否相等
- `override int GetHashCode()` - 获取哈希码

### Card (具体卡牌类)

```csharp
public sealed class Card : ACard, ICloneable<Card>
```

**属性:**
- `byte cost` - 卡牌费用
- `bool Xcost` - 是否为X费用卡牌
- `Target target` - 目标选择方式
- `IReadOnlyList<Box<string>?> Commands` - 卡牌命令列表

**方法:**
- `static Card Create(CardData data)` - 从CardData创建卡牌
- `static Card Create(string name, byte cost, bool Xcost, Rarity rarity, ITags tags, Target target, string commands)` - 创建卡牌
- `Card Clone()` - 克隆卡牌

### Material (材料卡牌类)

```csharp
public sealed class Material : ACard, ICloneable<Material>
```

**方法:**
- `Material Clone()` - 克隆材料卡牌

## 卡牌系统

### CardSystem

```csharp
public class CardSystem : MonoBehaviour
```

**属性:**
- `int CurrentMana` - 当前费用
- `int MaxMana` - 最大费用
- `int HandSize` - 手牌数量
- `int MaxHandSize` - 最大手牌数量

**方法:**
- `void AddCardToHand(CardView cardView)` - 添加卡牌到手牌
- `void RemoveCardFromHand(CardView cardView)` - 从手牌移除卡牌
- `bool PlayCard(CardView cardView, PlayArea playArea)` - 出牌
- `bool CanAffordCard(Card card)` - 检查是否有足够费用
- `void RestoreMana(int amount)` - 恢复费用
- `void SetMaxMana(int max)` - 设置最大费用
- `void ClearHand()` - 清空手牌
- `void ClearPlayedCards()` - 清空已出卡牌
- `List<CardView> GetHandCards()` - 获取手牌列表
- `List<CardView> GetPlayedCards()` - 获取已出卡牌列表

### CardManager

```csharp
public static class CardManager
```

**方法:**
- `static void Add(string id, Card card)` - 添加卡牌模板
- `static Card Get(string id)` - 获取卡牌模板
- `static void CostCast()` - 锁定卡牌管理器

## 实体系统

### Entity (实体基类)

```csharp
public class Entity : Properties
```

**属性:**
- `double GetMaxHealth()` - 获取最大生命值
- `double GetHealth()` - 获取当前生命值
- `bool IsAlive()` - 是否存活

**方法:**
- `void SetMaxHealth(double maxHealth)` - 设置最大生命值
- `void SetHealth(double health)` - 设置生命值
- `void ChangeMaxHealth(Process<double> process)` - 修改最大生命值
- `void ChangeHealth(Process<double> process)` - 修改生命值
- `bool ApplyDamage(double damage)` - 应用伤害
- `void SetAlive(bool alive)` - 设置存活状态
- `virtual void Die()` - 死亡时调用
- `virtual void Revive()` - 复活时调用

### Player (玩家类)

```csharp
public class Player : Entity
```

**属性:**
- `string Name` - 玩家名称
- `Slots<Card> Inventory` - 背包
- `Slots<Card> Situation` - 状况栏
- `Card? ArmorSlot` - 装备槽

**方法:**
- `void Play(uint index, Box<Entity>? targets)` - 出牌

### Combatant (战斗者类)

```csharp
public class Combatant : Entity
```

**属性:**
- `string Name` - 战斗者名称
- `double Health` - 生命值
- `double MaxHealth` - 最大生命值

## 属性系统

### Properties (属性管理器)

```csharp
public class Properties : ICloneable<Properties>
```

**存储方法:**
- `bool Store(string name, int value)` - 存储整数值
- `bool Store(string name, double value)` - 存储双精度值
- `bool Store(string name, bool value)` - 存储布尔值
- `bool Store(string name, string value)` - 存储字符串值
- `bool Store<T>(string name, T value) where T : notnull` - 存储引用类型值

**更新方法:**
- `bool Update(string name, int value)` - 更新整数值
- `bool Update(string name, double value)` - 更新双精度值
- `bool Update(string name, bool value)` - 更新布尔值
- `bool Update(string name, string value)` - 更新字符串值
- `bool Update<T>(string name, T value) where T : notnull` - 更新引用类型值

**获取方法:**
- `int? GetInt(string name, bool checkDefault = true)` - 获取整数值
- `double? GetDouble(string name, bool checkDefault = true)` - 获取双精度值
- `bool? GetBool(string name, bool checkDefault = true)` - 获取布尔值
- `string? GetString(string name, bool checkDefault = true)` - 获取字符串值
- `T? Get<T>(string name, bool checkDefault = true) where T : notnull` - 获取引用类型值

**修改方法:**
- `bool Change(string name, Process<int> processor)` - 修改整数值
- `bool Change(string name, Process<double> processor)` - 修改双精度值
- `bool Change(string name, Process<bool> processor)` - 修改布尔值
- `bool Change(string name, Process<string> processor)` - 修改字符串值
- `bool Change<T>(string name, Process<T> processor) where T : notnull` - 修改引用类型值

**其他方法:**
- `Properties Clone()` - 克隆属性管理器

## 事件系统

### EventManager (事件管理器)

```csharp
public static class EventManager
```

**属性:**
- `Events<IPriorityEvent, AConstPriorityEvent> Events` - 优先级事件
- `Events<ISlicedEvent, AConstSlicedEvent> SlicedEvents` - 分支事件
- `Events<IRandomEvent, AConstRandomEvent> RandomEvents` - 随机事件

**方法:**
- `static void BuildConst()` - 构建常量事件

### 优先级事件

```csharp
public interface IPriorityEvent
```

**方法:**
- `void Bind(Action action, int priority = 0)` - 绑定无参数事件
- `void Bind<T1>(Action<T1> action, int priority = 0)` - 绑定单参数事件
- `void Bind<T1, T2>(Action<T1, T2> action, int priority = 0)` - 绑定双参数事件
- `void Bind<T1, T2, T3>(Action<T1, T2, T3> action, int priority = 0)` - 绑定三参数事件
- `void Bind<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, int priority = 0)` - 绑定四参数事件
- `void Bind<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, int priority = 0)` - 绑定五参数事件
- `void Bind<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)` - 绑定六参数事件
- `void Bind<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)` - 绑定七参数事件
- `void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)` - 绑定八参数事件

### 分支事件

```csharp
public interface ISlicedEvent
```

**方法:**
- `void Bind(string branch, Action action, int priority = 0)` - 绑定分支事件
- `void Bind<T1>(string branch, Action<T1> action, int priority = 0)` - 绑定分支单参数事件
- `void Bind<T1, T2>(string branch, Action<T1, T2> action, int priority = 0)` - 绑定分支双参数事件
- `void Bind<T1, T2, T3>(string branch, Action<T1, T2, T3> action, int priority = 0)` - 绑定分支三参数事件
- `void Bind<T1, T2, T3, T4>(string branch, Action<T1, T2, T3, T4> action, int priority = 0)` - 绑定分支四参数事件
- `void Bind<T1, T2, T3, T4, T5>(string branch, Action<T1, T2, T3, T4, T5> action, int priority = 0)` - 绑定分支五参数事件
- `void Bind<T1, T2, T3, T4, T5, T6>(string branch, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)` - 绑定分支六参数事件
- `void Bind<T1, T2, T3, T4, T5, T6, T7>(string branch, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)` - 绑定分支七参数事件
- `void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string branch, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)` - 绑定分支八参数事件
- `void CreateBranch(string branch, Action? prepare = null, Action? finalize = null)` - 创建分支

### 随机事件

```csharp
public interface IRandomEvent
```

**方法:**
- `void Bind(string id, Action action, int priority = 0)` - 绑定随机事件
- `void Bind<T1>(string id, Action<T1> action, int priority = 0)` - 绑定随机单参数事件
- `void Bind<T1, T2>(string id, Action<T1, T2> action, int priority = 0)` - 绑定随机双参数事件
- `void Bind<T1, T2, T3>(string id, Action<T1, T2, T3> action, int priority = 0)` - 绑定随机三参数事件
- `void Bind<T1, T2, T3, T4>(string id, Action<T1, T2, T3, T4> action, int priority = 0)` - 绑定随机四参数事件
- `void Bind<T1, T2, T3, T4, T5>(string id, Action<T1, T2, T3, T4, T5> action, int priority = 0)` - 绑定随机五参数事件
- `void Bind<T1, T2, T3, T4, T5, T6>(string id, Action<T1, T2, T3, T4, T5, T6> action, int priority = 0)` - 绑定随机六参数事件
- `void Bind<T1, T2, T3, T4, T5, T6, T7>(string id, Action<T1, T2, T3, T4, T5, T6, T7> action, int priority = 0)` - 绑定随机七参数事件
- `void Bind<T1, T2, T3, T4, T5, T6, T7, T8>(string id, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, int priority = 0)` - 绑定随机八参数事件
- `void CreateItem(string id, uint weight = 1024, Action? prepare = null, Action? finalize = null)` - 创建随机项
- `void SetWeight(string id, uint weight)` - 设置权重

## 伤害系统

### Damage (伤害类)

```csharp
public class Damage
```

**属性:**
- `string type` - 伤害类型
- `double value` - 伤害值

**方法:**
- `void AddModifier(string mdfid, Modifier<Damage> mdf, bool replaceTags = true, bool mergeTags = false)` - 添加修改器
- `void AddModifier(string mdfid, Func<Process<double>, int, ITags, DamageModifier> creator, Process<double> processer, int priority, string? tags = null)` - 添加修改器(优化版)
- `void AddModifier<T>(string mdfid, Func<T, int, ITags, DamageModifier> creator, T value, int priority, string? tags = null)` - 添加修改器(泛型版)
- `void UpdateModifier(string mdfid, Modifier<Damage> mdf)` - 更新修改器
- `bool RemoveModifier(string mdfid)` - 移除修改器
- `double Get()` - 获取最终伤害值

### DamageModifiers (伤害修改器工厂)

```csharp
public static class DamageModifiers
```

**方法:**
- `static DamageModifierAdd CreateAdd(double value, int priority, ITags tags)` - 创建加法修改器
- `static DamageModifierAdd CreateAdd(double value, int priority)` - 创建加法修改器(无标签)
- `static DamageModifierMul CreateMul(double value, int priority, ITags tags)` - 创建乘法修改器
- `static DamageModifierMul CreateMul(double value, int priority)` - 创建乘法修改器(无标签)
- `static DamageModifierMulTotal CreateMulTotal(double value, int priority, ITags tags)` - 创建总乘法修改器
- `static DamageModifierMulTotal CreateMulTotal(double value, int priority)` - 创建总乘法修改器(无标签)
- `static DamageModifierCustom CreateCustom(Process<double> processer, int priority, ITags tags)` - 创建自定义修改器
- `static DamageModifierCustom CreateCustom(Process<double> processer, int priority)` - 创建自定义修改器(无标签)

### DamageTags (伤害标签)

```csharp
public static class DamageTags
```

**方法:**
- `static bool Ignore(string type, ITags tags)` - 检查伤害类型是否被忽略
- `static bool Ignore(string type, string tag)` - 检查伤害类型是否被忽略(单标签)

## 拖拽系统

### CardDragSystem (卡牌拖拽系统)

```csharp
public class CardDragSystem : Singleton<CardDragSystem>
```

**属性:**
- `bool IsDragging` - 是否正在拖拽
- `CardView DraggedCard` - 当前拖拽的卡牌

**方法:**
- `void StartDrag(CardView cardView)` - 开始拖拽
- `void EndDrag()` - 结束拖拽
- `void CancelDrag()` - 取消拖拽
- `void RegisterPlayArea(IPlayArea playArea)` - 注册出牌区域
- `void UnregisterPlayArea(IPlayArea playArea)` - 注销出牌区域

### PlayArea (出牌区域)

```csharp
public class PlayArea : MonoBehaviour, IPlayArea
```

**方法:**
- `bool CanPlayCard(CardView cardView)` - 检查是否可以出牌
- `bool TryPlayCard(CardView cardView)` - 尝试出牌
- `void SetHighlight(bool highlight)` - 设置高亮
- `void RemoveCard(CardView cardView)` - 移除卡牌
- `void ClearCards()` - 清空所有卡牌
- `int GetCardCount()` - 获取当前卡牌数量
- `void SetMaxCards(int max)` - 设置最大卡牌数量
- `void SetAllowedCardTypes(string[] types)` - 设置允许的卡牌类型

### CardDragBehavior (卡牌拖拽行为)

```csharp
public class CardDragBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
```

**方法:**
- `void SetDragEnabled(bool enabled)` - 设置是否启用拖拽
- `bool IsDragEnabled()` - 获取是否启用拖拽
- `void CancelDrag()` - 强制取消拖拽

### IPlayArea (出牌区域接口)

```csharp
public interface IPlayArea
```

**方法:**
- `bool CanPlayCard(CardView cardView)` - 检查是否可以出牌
- `bool TryPlayCard(CardView cardView)` - 尝试出牌
- `void SetHighlight(bool highlight)` - 设置高亮

## 工具类

### Parser (字符串解析器)

```csharp
public static class Parser
```

**方法:**
- `static IEnumerable<object>? ToCollection(string src, byte limit = 255, bool strict = false)` - 解析为集合
- `static Box<string>?[]? ToBoxArray(string src, bool strict = false)` - 解析为Box数组
- `static Box<string>? ToBox(string src, bool strict = false)` - 解析为Box

### DragSystemSetup (拖拽系统设置工具)

```csharp
public class DragSystemSetup : MonoBehaviour
```

**方法:**
- `void SetupDragSystem()` - 设置拖拽系统
- `void CleanupDragSystem()` - 清理拖拽系统

### Singleton<T> (单例基类)

```csharp
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
```

**属性:**
- `static T Instance` - 单例实例

**方法:**
- `protected virtual void Awake()` - 初始化方法

## 枚举类型

### Rarity (稀有度)

```csharp
public enum Rarity : byte
{
    Common = 0,    // 普通
    Uncommon = 1,  // 不凡
    Rare = 2,      // 稀有
    Epic = 3,      // 史诗
    Legend = 4,    // 传说
    Unique = 5     // 唯一
}
```

### Target (目标选择)

```csharp
public enum Target : byte
{
    Selected = 0,  // 指定实体
    Enemy = 1,     // 敌方实体
    All = 2,       // 所有实体
    Random = 3,    // 随机实体
    Self = 4       // 自身
}
```

## 委托类型

### Process<T> (处理委托)

```csharp
public delegate void Process<T>(ref T value);
```

## 使用示例

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
EventManager.Events["CardPlayed"].Bind((Entity player, Card card) => {
    Debug.Log($"{player.Name} 打出了 {card.Name}");
});

// 触发事件
EventManager.Events["CardPlayed"].Trigger((player, card));
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

---

**注意**: 此API文档基于当前代码版本，如有更新请参考最新代码。
