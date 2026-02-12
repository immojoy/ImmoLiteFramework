# ImmoLiteFramework 编码规范

## 目录
1. [命名约定](#命名约定)
2. [代码组织](#代码组织)
3. [命名空间](#命名空间)
4. [类设计](#类设计)
5. [字段和属性](#字段和属性)
6. [方法和参数](#方法和参数)
7. [注释和文档](#注释和文档)
8. [Unity特定规范](#unity特定规范)
9. [设计模式](#设计模式)
10. [错误处理](#错误处理)

---

## 命名约定

### 类和结构体
- **格式**: `PascalCase`
- **前缀**: 所有框架核心类使用 `Immo` 前缀
- **示例**:
  ```csharp
  public class ImmoUiManager : MonoBehaviour
  public class ImmoEventManager : MonoBehaviour
  public class ImmoResourceManager : MonoBehaviour
  ```

### 接口
- **格式**: `PascalCase`，使用 `I` 前缀
- **内部接口**: 使用 `internal` 访问修饰符
- **示例**:
  ```csharp
  internal interface IImmoEventHandler
  internal interface IImmoEventHandler<T> : IImmoEventHandler where T : ImmoEvent
  ```

### 枚举
- **格式**: `PascalCase`（枚举类型和枚举值）
- **前缀**: 枚举类型不使用前缀
- **示例**:
  ```csharp
  public enum UiLayer
  {
      None,
      Background,
      Normal,
      Popup,
      Top,
      System
  }
  
  public enum ImmoEventHandlerPriority
  {
      Lowest = 0,
      Low = 1,
      Normal = 2,
      High = 3,
      Highest = 4
  }
  ```

### 字段
- **私有字段**: 使用 `m_` 前缀 + `PascalCase`
- **常量**: 使用 `PascalCase`（如果是局部常量）
- **示例**:
  ```csharp
  private static ImmoUiManager m_Instance;
  private Dictionary<string, ImmoUiView> m_CachedViews = new();
  private Dictionary<UiLayer, Transform> m_LayerRoots = new();
  [SerializeField] private ImmoUiLayerConfig m_LayerConfig;
  private readonly Dictionary<UiLayer, int> m_DefaultLayerSortOrders = new();
  private readonly object m_Lock = new();
  ```

### 属性
- **格式**: `PascalCase`
- **公共属性**: 无前缀
- **示例**:
  ```csharp
  public static ImmoUiManager Instance => m_Instance;
  public bool IsCancelled { get; private set; }
  public DateTime Timestamp { get; private set; }
  public object Source { get; private set; }
  public virtual ImmoEventHandlerPriority Priority { get; } = ImmoEventHandlerPriority.Normal;
  ```

### 方法
- **格式**: `PascalCase`
- **命名原则**: 使用动词或动词短语描述操作
- **示例**:
  ```csharp
  public void ShowUi(string assetAddress, object args = null)
  public void HideUi(string assetAddress)
  public void TriggerEvent<T>(T e) where T : ImmoEvent
  public void RegisterHandler<T>(ImmoEventHandler<T> handler) where T : ImmoEvent
  private void InitializeLayers()
  private void SetupLayerSortOrder()
  ```

### 参数
- **格式**: `camelCase`
- **示例**:
  ```csharp
  public void LoadAssetAsyncWithCallback<T>(string assetAddress, OnAssetLoadSuccess successCallback, object data)
  private void OnUiLoadSuccess(string address, object uiPrefab, float duration, object args)
  public UiLayerAttribute(UiLayer layerType) => LayerType = layerType;
  ```

### 委托
- **格式**: `PascalCase`
- **前缀**: 回调委托使用 `On` 前缀
- **示例**:
  ```csharp
  public delegate void OnAssetLoadSuccess(string assetName, object asset, float duration, object userData);
  ```

---

## 代码组织

### 文件结构
1. Using 语句（按字母顺序，System 命名空间优先）
2. 空行
3. 命名空间声明
4. 类/结构体/枚举定义

**示例**:
```csharp
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Immojoy.LiteFramework.Runtime
{
    public class ImmoUiManager : MonoBehaviour
    {
        // 类实现
    }
}
```

### 类成员顺序
1. 常量字段
2. 静态字段
3. 实例字段（序列化字段优先）
4. 属性
5. Unity 生命周期方法（Awake, Start, Update等）
6. 公共方法
7. 私有方法
8. 嵌套类型

**示例**:
```csharp
public class ImmoUiManager : MonoBehaviour
{
    // 1. 序列化字段
    [Header("Configuration")]
    [SerializeField] private ImmoUiLayerConfig m_LayerConfig;
    
    // 2. 静态字段
    private static ImmoUiManager m_Instance;
    
    // 3. 静态属性
    public static ImmoUiManager Instance => m_Instance;

    // 4. 实例字段
    private Dictionary<string, ImmoUiView> m_CachedViews = new();
    private readonly Dictionary<UiLayer, int> m_DefaultLayerSortOrders = new();
    
    // 5. 公共方法
    public void ShowUi(string assetAddress, object args = null) { }
    public void HideUi(string assetAddress) { }
    
    // 6. 私有方法
    private void InitializeLayers() { }
    private void OnUiLoadSuccess(...) { }
}
```

---

## 命名空间

### 标准命名空间
- **根命名空间**: `Immojoy.LiteFramework`
- **运行时代码**: `Immojoy.LiteFramework.Runtime`
- **编辑器代码**: `Immojoy.LiteFramework.Editor`

**示例**:
```csharp
namespace Immojoy.LiteFramework.Runtime
{
    // 运行时代码
}

namespace Immojoy.LiteFramework.Editor
{
    // 编辑器代码
}
```

---

## 类设计

### 单例模式
- 使用静态字段 `m_Instance` 和静态属性 `Instance`
- 在 `Awake` 中实现单例检查
- **示例**:
  ```csharp
  private static ImmoEventManager m_Instance;
  public static ImmoEventManager Instance => m_Instance;

  private void Awake()
  {
      if (m_Instance != null && m_Instance != this)
      {
          Destroy(this);
          return;
      }
      
      m_Instance = this;
  }
  ```

### 抽象类和虚方法
- 使用 `abstract` 关键字定义抽象类和必须实现的方法
- 使用 `virtual` 关键字定义可选重写的方法
- **示例**:
  ```csharp
  public abstract class ImmoUiView : MonoBehaviour
  {
      public virtual void OnCreate() { }
      public virtual void OnShow(object args = null) => gameObject.SetActive(true);
      public virtual void OnHide() => gameObject.SetActive(false);
      public virtual void OnDestroy() => Destroy(gameObject);
  }
  
  public abstract class ImmoEvent
  {
      protected virtual bool IsCancellable()
      {
          return true;
      }
  }
  ```

### 泛型类型约束
- 明确指定泛型约束
- **示例**:
  ```csharp
  public void LoadAssetAsyncWithCallback<T>(string assetAddress, OnAssetLoadSuccess successCallback, object data) 
      where T : UnityEngine.Object
  
  public abstract class ImmoEventHandler<T> : IImmoEventHandler<T> 
      where T : ImmoEvent
  ```

---

## 字段和属性

### 字段声明
- 私有字段使用 `m_` 前缀
- 只读字段使用 `readonly` 修饰符
- 使用集合初始化器（推荐 C# 9.0+ 的 `new()` 语法）
- **示例**:
  ```csharp
  private Dictionary<string, ImmoUiView> m_CachedViews = new();
  private readonly Dictionary<Type, List<IImmoEventHandler>> m_EventHandlers = new();
  private readonly object m_Lock = new();
  ```

### 序列化字段
- 使用 `[SerializeField]` 特性暴露私有字段
- 添加 `[Header]` 和 `[Tooltip]` 提供编辑器提示
- **示例**:
  ```csharp
  [Header("Configuration")]
  [Tooltip("Optional: Custom UI layer configuration. If not set, default values will be used.")]
  [SerializeField] private ImmoUiLayerConfig m_LayerConfig;
  
  [Header("UI Layer Sort Orders")]
  [Tooltip("Sort order for Background layer (lowest priority)")]
  [SerializeField] private int m_BackgroundSortOrder = 0;
  ```

### 属性
- 优先使用表达式主体属性（Expression-bodied properties）
- 公共属性使用 `PascalCase`
- **示例**:
  ```csharp
  public static ImmoUiManager Instance => m_Instance;
  public bool IsCancelled { get; private set; }
  public Type EventType => typeof(T);
  public virtual ImmoEventHandlerPriority Priority { get; } = ImmoEventHandlerPriority.Normal;
  ```

---

## 方法和参数

### 方法签名
- 可选参数使用默认值
- 参数使用 `camelCase`
- **示例**:
  ```csharp
  public void ShowUi(string assetAddress, object args = null)
  public virtual void OnShow(object args = null) => gameObject.SetActive(true);
  ```

### 表达式主体方法
- 简单方法使用表达式主体
- **示例**:
  ```csharp
  public virtual void OnShow(object args = null) => gameObject.SetActive(true);
  public virtual void OnHide() => gameObject.SetActive(false);
  public virtual void OnDestroy() => Destroy(gameObject);
  public UiLayerAttribute(UiLayer layerType) => LayerType = layerType;
  ```

### Switch 表达式
- 使用现代 C# switch 表达式（C# 8.0+）
- **示例**:
  ```csharp
  public int GetSortOrder(UiLayer layer)
  {
      return layer switch
      {
          UiLayer.Background => m_BackgroundSortOrder,
          UiLayer.Normal => m_NormalSortOrder,
          UiLayer.Popup => m_PopupSortOrder,
          UiLayer.Top => m_TopSortOrder,
          UiLayer.System => m_SystemSortOrder,
          _ => 0
      };
  }
  ```

---

## 注释和文档

### XML 文档注释
- 所有公共 API 必须添加 XML 注释
- 使用 `<summary>` 描述功能
- 使用 `<param>` 描述参数
- 使用 `<returns>` 描述返回值
- **示例**:
  ```csharp
  /// <summary>
  /// Registers an event handler for a specific event type.
  /// </summary>
  /// <param name="handler">The event handler to register.</param>
  public void RegisterHandler<T>(ImmoEventHandler<T> handler) where T : ImmoEvent
  
  /// <summary>
  /// Triggers an event to be processed.</br>
  /// This method queues the event for processing in the next update cycle.
  /// </summary>
  /// <param name="e">The event to trigger.</param>
  public void TriggerEvent<T>(T e) where T : ImmoEvent
  
  /// <summary>
  /// Determines whether the event can be cancelled.    
  /// </summary>
  /// <returns><b>true</b> if the event can be cancelled; otherwise, <b>false</b>.</returns>
  protected virtual bool IsCancellable()
  ```

### 行内注释
- 使用 `//` 进行简短说明
- 使用 TODO 标记待完成功能
- **示例**:
  ```csharp
  // TODO: Currently, only singular views are supported. Consider adding support for multiple instances if needed.
  
  // Set Canvas sort order for each layer to ensure proper rendering priority
  SetupLayerSortOrder();
  
  // Check for ongoing loading of the same asset to avoid duplication
  if (m_OngoingCallbacks.TryGetValue(assetAddress, out List<OnAssetLoadSuccess> callbacks))
  ```

---

## Unity特定规范

### Unity Attributes
- 使用 `[DisallowMultipleComponent]` 防止重复添加组件
- 使用 `[AddComponentMenu]` 提供友好的组件菜单路径
- 使用 `[CreateAssetMenu]` 为 ScriptableObject 提供创建菜单
- **示例**:
  ```csharp
  [DisallowMultipleComponent]
  [AddComponentMenu("Immojoy/Lite Framework/Manager/Immo UI Manager")]
  public class ImmoUiManager : MonoBehaviour
  
  [DisallowMultipleComponent]
  [AddComponentMenu("Immojoy/Lite Framework/Manager/Immo Event Manager")]
  public sealed class ImmoEventManager : MonoBehaviour
  
  [CreateAssetMenu(fileName = "UiLayerConfig", menuName = "Immojoy/Lite Framework/UI Layer Config")]
  public class ImmoUiLayerConfig : ScriptableObject
  ```

### Unity 生命周期
- 按 Unity 调用顺序组织生命周期方法
- 顺序: `Awake` → `OnEnable` → `Start` → `Update` → `OnDisable` → `OnDestroy`
- **示例**:
  ```csharp
  private void Awake()
  {
      if (m_Instance != null && m_Instance != this)
      {
          Destroy(this);
      }
      else
      {
          m_Instance = this;
      }
  }
  ```

### ScriptableObject 验证
- 使用 `OnValidate` 方法进行数据验证
- **示例**:
  ```csharp
  private void OnValidate()
  {
      if (m_BackgroundSortOrder >= m_NormalSortOrder ||
          m_NormalSortOrder >= m_PopupSortOrder ||
          m_PopupSortOrder >= m_TopSortOrder ||
          m_TopSortOrder >= m_SystemSortOrder)
      {
          Debug.LogWarning("UI Layer sort orders should be in ascending order: Background < Normal < Popup < Top < System");
      }
  }
  ```

### 编辑器扩展
- 使用 `MenuItem` 创建菜单项
- 使用 `Undo` 系统支持撤销操作
- 使用 `GameObjectUtility.SetParentAndAlign` 设置父级
- **示例**:
  ```csharp
  [MenuItem("GameObject/Immo Lite Framework/Framework", false, 10)]
  private static void CreateFramework(MenuCommand menuCommand)
  {
      GameObject frameworkRoot = new GameObject("Immo Lite Framework");
      Undo.RegisterCreatedObjectUndo(frameworkRoot, "Create Immo Lite Framework");
      GameObjectUtility.SetParentAndAlign(frameworkRoot, menuCommand.context as GameObject);
      Selection.activeObject = frameworkRoot;
  }
  ```

---

## 设计模式

### 单例模式（Singleton）
- 所有 Manager 类使用单例模式
- 使用静态实例和属性访问
- **示例**: `ImmoUiManager`, `ImmoEventManager`, `ImmoResourceManager`

### 事件系统
- 基于泛型的类型安全事件系统
- 事件处理器具有优先级
- 支持事件取消机制
- **核心类**:
  - `ImmoEvent`: 事件基类
  - `ImmoEventHandler<T>`: 事件处理器基类
  - `ImmoEventManager`: 事件管理器

### 回调模式
- 使用委托定义回调
- **示例**:
  ```csharp
  public delegate void OnAssetLoadSuccess(string assetName, object asset, float duration, object userData);
  
  public void LoadAssetAsyncWithCallback<T>(string assetAddress, OnAssetLoadSuccess successCallback, object data)
  ```

### 生命周期管理
- UI 视图使用标准生命周期方法
- **生命周期方法**: `OnCreate` → `OnShow` → `OnHide` → `OnDestroy`
- **示例**:
  ```csharp
  public abstract class ImmoUiView : MonoBehaviour
  {
      public virtual void OnCreate() { }
      public virtual void OnShow(object args = null) => gameObject.SetActive(true);
      public virtual void OnHide() => gameObject.SetActive(false);
      public virtual void OnDestroy() => Destroy(gameObject);
  }
  ```

---

## 错误处理

### 参数验证
- 验证必需参数不为空
- 使用 `ArgumentException` 抛出异常
- **示例**:
  ```csharp
  public void LoadAssetAsyncWithCallback<T>(string assetAddress, OnAssetLoadSuccess successCallback, object data)
  {
      if (string.IsNullOrEmpty(assetAddress))
      {
          throw new ArgumentException("Asset address cannot be null or empty.", nameof(assetAddress));
      }
      // ...
  }
  ```

### 空值检查
- 使用空合并运算符 `??` 提供默认值
- 使用可空引用类型 `?` 表示可选值
- **示例**:
  ```csharp
  UiLayer layer = GetUiLayer(uiView.GetType()) ?? UiLayer.Normal;  // Fallback to default layer
  successCallback?.Invoke(assetAddress, result, 0, data);
  ```

### 日志记录
- 使用 `Debug.LogError` 记录错误
- 使用 `Debug.LogWarning` 记录警告
- **示例**:
  ```csharp
  if (uiView != null)
  {
      // 正常处理
  }
  else
  {
      Debug.LogError($"The loaded UI prefab at {address} does not have an ImmoUiView component.");
      Destroy(uiObject);
  }
  
  Debug.LogWarning("UI Layer sort orders should be in ascending order: Background < Normal < Popup < Top < System");
  ```

### 条件检查
- 使用早期返回（Early Return）减少嵌套
- **示例**:
  ```csharp
  public void TriggerEvent<T>(T e) where T : ImmoEvent
  {
      if (e == null || e.IsCancelled)
      {
          return;
      }
      
      lock (m_Lock)
      {
          m_EventQueue.Enqueue(e);
      }
  }
  ```

---

## 代码风格总结

### ✅ 推荐做法
- 使用现代 C# 语法（目标初始化器、switch 表达式、表达式主体成员）
- 为所有公共 API 添加 XML 文档注释
- 使用 `readonly` 修饰符标记不可变字段
- 使用泛型提高类型安全性
- 使用 `sealed` 密封不应被继承的类
- 使用 Unity Attributes 增强编辑器体验
- 使用 `private` 访问修饰符保护内部实现

### ❌ 避免做法
- 避免使用魔法数字，使用命名常量或配置
- 避免过度嵌套，使用早期返回
- 避免在公共 API 中暴露实现细节
- 避免使用过时的 C# 语法

### 📝 命名规则快速参考
| 类型 | 规则 | 示例 |
|------|------|------|
| 类 | `PascalCase`，使用 `Immo` 前缀 | `ImmoUiManager` |
| 接口 | `PascalCase`，使用 `I` 前缀 | `IImmoEventHandler` |
| 枚举 | `PascalCase` | `UiLayer`, `ImmoEventHandlerPriority` |
| 私有字段 | `m_PascalCase` | `m_Instance`, `m_CachedViews` |
| 公共属性 | `PascalCase` | `Instance`, `IsCancelled` |
| 方法 | `PascalCase` | `ShowUi`, `RegisterHandler` |
| 参数 | `camelCase` | `assetAddress`, `layerType` |
| 委托 | `PascalCase`，回调使用 `On` 前缀 | `OnAssetLoadSuccess` |

---

## 版本信息
- **文档版本**: 1.0
- **框架版本**: 0.0.3
- **创建日期**: 2026-01-29
- **适用于**: Unity 2021.3+, C# 9.0+

---

## 参考资料
- [Microsoft C# 编码规范](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Unity 脚本最佳实践](https://unity.com/how-to/programming-unity)
