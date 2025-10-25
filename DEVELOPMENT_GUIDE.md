# MineCombat 开发指南

## 开发环境设置

### 必需软件
- Unity 2022.3 或更高版本
- Visual Studio 2022 或 JetBrains Rider
- Git 版本控制
- .NET Standard 2.1

### 项目配置
1. 克隆项目到本地
2. 使用Unity Hub打开项目
3. 等待Unity自动导入资源
4. 配置IDE的代码补全和调试

## 代码规范

### 命名规范

#### 类名
- 使用PascalCase命名
- 类名应该清晰表达其用途
```csharp
public class CardDragSystem : Singleton<CardDragSystem>
public class DamageModifierAdd : DamageModifier
```

#### 方法名
- 使用PascalCase命名
- 方法名应该清晰表达其功能
```csharp
public void StartDrag(CardView cardView)
public bool CanPlayCard(CardView cardView)
```

#### 变量名
- 使用camelCase命名
- 私有字段使用下划线前缀
```csharp
private bool _isDragging;
private CardView _draggedCard;
public int currentMana;
```

#### 常量
- 使用PascalCase命名
- 常量应该全大写
```csharp
public const int MAX_HAND_SIZE = 10;
public const string CARD_PLAYED_EVENT = "CardPlayed";
```

### 代码结构

#### 类结构顺序
1. 字段声明
2. 属性声明
3. 构造函数
4. 公共方法
5. 受保护方法
6. 私有方法
7. 事件处理

```csharp
public class ExampleClass
{
    // 1. 字段声明
    private int _value;
    private bool _isActive;
    
    // 2. 属性声明
    public int Value { get; set; }
    public bool IsActive { get; private set; }
    
    // 3. 构造函数
    public ExampleClass(int value)
    {
        _value = value;
        IsActive = true;
    }
    
    // 4. 公共方法
    public void DoSomething()
    {
        // 实现
    }
    
    // 5. 受保护方法
    protected virtual void OnValueChanged()
    {
        // 实现
    }
    
    // 6. 私有方法
    private void InternalMethod()
    {
        // 实现
    }
}
```

### 注释规范

#### XML文档注释
```csharp
/// <summary>
/// 卡牌拖拽系统，管理所有卡牌的拖拽逻辑
/// </summary>
public class CardDragSystem : Singleton<CardDragSystem>
{
    /// <summary>
    /// 开始拖拽卡牌
    /// </summary>
    /// <param name="cardView">要拖拽的卡牌视图</param>
    public void StartDrag(CardView cardView)
    {
        // 实现
    }
}
```

#### 行内注释
```csharp
// 检查是否在出牌区域内
foreach (var playArea in playAreas)
{
    bool canPlay = playArea.CanPlayCard(draggedCard);
    playArea.SetHighlight(canPlay);
}
```

## 开发流程

### 1. 功能开发

#### 创建新功能
1. 在适当的模块中创建新类
2. 实现必要的接口
3. 添加单元测试
4. 更新文档

#### 修改现有功能
1. 理解现有代码结构
2. 进行最小化修改
3. 确保向后兼容性
4. 更新相关测试

### 2. 测试策略

#### 单元测试
```csharp
[Test]
public void TestCardCreation()
{
    // Arrange
    var cardData = new CardData
    {
        Name = "Test Card",
        cost = 3,
        rarity = "Rare"
    };
    
    // Act
    var card = Card.Create(cardData);
    
    // Assert
    Assert.AreEqual("Test Card", card.Name);
    Assert.AreEqual(3, card.cost);
    Assert.AreEqual(Rarity.Rare, card.rarity);
}
```

#### 集成测试
```csharp
[Test]
public void TestCardPlayFlow()
{
    // Arrange
    var player = new Player("TestPlayer", 100.0);
    var card = Card.Create("Test Card", 3, false, Rarity.Common, 
        new Tags(), Target.Self, "heal:10");
    var playArea = new PlayArea();
    
    // Act
    bool result = playArea.TryPlayCard(cardView);
    
    // Assert
    Assert.IsTrue(result);
    Assert.AreEqual(110.0, player.GetHealth());
}
```

### 3. 调试技巧

#### 日志记录
```csharp
// 使用Debug.Log记录重要信息
Debug.Log($"开始拖拽卡牌: {cardView.Card?.Name}");

// 使用Debug.LogWarning记录警告
Debug.LogWarning("费用不足，无法出牌");

// 使用Debug.LogError记录错误
Debug.LogError("卡牌数据无效");
```

#### 断点调试
- 在关键方法设置断点
- 使用条件断点
- 检查变量值
- 单步执行代码

#### 性能分析
```csharp
// 使用Unity Profiler分析性能
using UnityEngine.Profiling;

Profiler.BeginSample("CardDragSystem.UpdateDragPosition");
// 性能关键代码
Profiler.EndSample();
```

## 常见问题解决

### 1. 编译错误

#### 命名空间问题
```csharp
// 确保使用正确的命名空间
using MineCombat;
using UnityEngine;
```

#### 类型转换问题
```csharp
// 安全的类型转换
if (obj is Card card)
{
    // 使用card
}

// 或者使用as操作符
var card = obj as Card;
if (card != null)
{
    // 使用card
}
```

### 2. 运行时错误

#### 空引用异常
```csharp
// 使用空条件操作符
var name = cardView?.Card?.Name ?? "Unknown";

// 使用空合并操作符
var value = GetInt("attack") ?? 0;
```

#### 数组越界
```csharp
// 检查数组边界
if (index >= 0 && index < array.Length)
{
    var item = array[index];
}
```

### 3. 性能问题

#### 频繁的GC分配
```csharp
// 避免在Update中创建新对象
private List<CardView> _tempList = new List<CardView>();

void Update()
{
    _tempList.Clear();
    // 使用_tempList而不是创建新List
}
```

#### 过度的字符串操作
```csharp
// 使用StringBuilder进行大量字符串操作
StringBuilder sb = new StringBuilder();
sb.Append("Card: ");
sb.Append(card.Name);
sb.Append(", Cost: ");
sb.Append(card.cost);
string result = sb.ToString();
```

## 代码审查清单

### 1. 代码质量
- [ ] 代码遵循命名规范
- [ ] 方法长度适中（< 50行）
- [ ] 类职责单一
- [ ] 无重复代码
- [ ] 适当的注释

### 2. 性能
- [ ] 无不必要的内存分配
- [ ] 算法复杂度合理
- [ ] 避免频繁的字符串操作
- [ ] 使用对象池（如适用）

### 3. 安全性
- [ ] 空引用检查
- [ ] 边界条件处理
- [ ] 异常处理
- [ ] 线程安全（如适用）

### 4. 可维护性
- [ ] 代码结构清晰
- [ ] 依赖关系合理
- [ ] 易于测试
- [ ] 文档完整

## 版本控制

### 分支策略
- `main`: 主分支，包含稳定版本
- `develop`: 开发分支，包含最新开发内容
- `feature/*`: 功能分支，用于开发新功能
- `bugfix/*`: 修复分支，用于修复bug
- `hotfix/*`: 热修复分支，用于紧急修复

### 提交信息规范
```
<类型>(<范围>): <描述>

<详细描述>

<相关issue>
```

#### 类型
- `feat`: 新功能
- `fix`: 修复bug
- `docs`: 文档更新
- `style`: 代码格式调整
- `refactor`: 代码重构
- `test`: 测试相关
- `chore`: 构建过程或辅助工具的变动

#### 示例
```
feat(card): 添加卡牌拖拽功能

- 实现CardDragSystem类
- 添加CardDragBehavior组件
- 支持拖拽到出牌区域

Closes #123
```

## 部署指南

### 1. 构建配置
- 设置正确的构建设置
- 配置代码优化选项
- 设置适当的压缩级别

### 2. 测试验证
- 运行所有测试
- 进行手动测试
- 检查性能指标

### 3. 发布流程
1. 更新版本号
2. 更新CHANGELOG
3. 创建发布标签
4. 构建发布版本
5. 部署到目标平台

## 贡献指南

### 1. 如何贡献
1. Fork项目
2. 创建功能分支
3. 提交更改
4. 创建Pull Request

### 2. Pull Request要求
- 代码质量符合标准
- 包含必要的测试
- 更新相关文档
- 通过所有检查

### 3. 问题报告
- 使用问题模板
- 提供复现步骤
- 包含环境信息
- 添加相关日志

---

**注意**: 此开发指南会随着项目发展持续更新，请关注最新版本。
