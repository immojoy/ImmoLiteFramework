# 流程模块 (Procedure)

## 概述

ImmoLiteFramework 的流程模块基于 GameFramework 的设计思想，提供了一个用于管理游戏整个生命周期的流程系统。流程本质上是一个有限状态机，用于解耦游戏的不同阶段。

## 什么是流程？

流程（Procedure）是贯穿游戏运行时整个生命周期的有限状态机。通过流程，将不同的游戏状态进行解耦是一个非常好的习惯。

**典型的游戏流程**：
- **网络游戏**：启动流程 → 检查资源流程 → 更新资源流程 → 登录流程 → 选择服务器流程 → 创建角色流程 → 游戏流程
- **单机游戏**：启动流程 → 主菜单流程 → 游戏玩法流程 → 暂停流程 → 结算流程

## 核心特性

- **生命周期管理**：基于状态机的完整生命周期（OnInit, OnEnter, OnUpdate, OnLeave, OnDestroy）
- **数据共享**：流程间可以通过状态机数据系统共享信息
- **解耦设计**：每个流程独立管理自己的逻辑，互不干扰
- **易于扩展**：只需继承 `ImmoProcedureBase` 即可添加新流程

## 核心类

### 1. `ImmoProcedureBase`
流程基类，所有自定义流程都需要继承此类。

**生命周期方法**：
- `OnInit(IImmoFsm<ImmoProcedureManager> procedureOwner)` - 流程初始化时调用一次
- `OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)` - 每次进入流程时调用
- `OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)` - 流程运行时每帧调用
- `OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)` - 每次离开流程时调用
- `OnDestroy(IImmoFsm<ImmoProcedureManager> procedureOwner)` - 流程销毁时调用一次

**流程切换**：
- `ChangeState<T>(IImmoFsm<ImmoProcedureManager> procedureOwner)` - 切换到指定流程

### 2. `ImmoProcedureManager`
流程管理器，负责管理所有流程。

**单例访问**：
```csharp
ImmoProcedureManager.Instance
```

**核心属性**：
- `CurrentProcedure` - 当前流程
- `CurrentProcedureTime` - 当前流程持续时间

**核心方法**：
- `Initialize(params ImmoProcedureBase[] procedures)` - 初始化流程管理器
- `StartProcedure<T>()` - 启动指定流程
- `HasProcedure<T>()` - 检查流程是否存在
- `GetProcedure<T>()` - 获取指定流程

## 使用示例

### 1. 定义流程

```csharp
/// <summary>
/// 启动流程。
/// </summary>
public class LaunchProcedure : ImmoProcedureBase
{
    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.Log("进入启动流程");
        
        // 设置游戏版本
        procedureOwner.SetData("GameVersion", "1.0.0");
    }

    protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        // 启动完成后切换到主菜单
        if (procedureOwner.CurrentStateTime >= 3f)
        {
            Debug.Log("启动完成，切换到主菜单");
            ChangeState<MainMenuProcedure>(procedureOwner);
        }
    }

    protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        Debug.Log("离开启动流程");
    }
}

/// <summary>
/// 主菜单流程。
/// </summary>
public class MainMenuProcedure : ImmoProcedureBase
{
    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.Log("进入主菜单流程");
        
        // 显示主菜单 UI
        // ImmoUiManager.Instance.ShowUi("MainMenuUI");
    }

    protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        // 检测开始游戏
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeState<GamePlayProcedure>(procedureOwner);
        }
    }

    protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        
        // 隐藏主菜单 UI
        // ImmoUiManager.Instance.HideUi("MainMenuUI");
    }
}

/// <summary>
/// 游戏流程。
/// </summary>
public class GamePlayProcedure : ImmoProcedureBase
{
    private float m_GameTime;

    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.Log("进入游戏流程");
        
        m_GameTime = 0f;
        procedureOwner.SetData("GameStartTime", Time.time);
    }

    protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        m_GameTime += elapseSeconds;
        
        // 游戏逻辑...
        
        // 按 ESC 暂停游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState<PauseProcedure>(procedureOwner);
        }
    }

    protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        Debug.Log($"离开游戏流程，游戏时长：{m_GameTime:F2}秒");
        
        procedureOwner.RemoveData("GameStartTime");
    }
}
```

### 2. 初始化和启动流程

```csharp
public class GameEntry : MonoBehaviour
{
    private void Start()
    {
        // 创建流程实例
        LaunchProcedure launchProcedure = new LaunchProcedure();
        MainMenuProcedure mainMenuProcedure = new MainMenuProcedure();
        GamePlayProcedure gamePlayProcedure = new GamePlayProcedure();
        PauseProcedure pauseProcedure = new PauseProcedure();

        // 初始化流程管理器
        ImmoProcedureManager.Instance.Initialize(
            launchProcedure,
            mainMenuProcedure,
            gamePlayProcedure,
            pauseProcedure
        );

        // 启动首个流程
        ImmoProcedureManager.Instance.StartProcedure<LaunchProcedure>();
    }
}
```

### 3. 流程间数据共享

```csharp
public class LoginProcedure : ImmoProcedureBase
{
    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        // 设置玩家信息
        procedureOwner.SetData("PlayerName", "Player001");
        procedureOwner.SetData("PlayerId", 12345);
    }

    protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        if (IsLoginSuccess())
        {
            ChangeState<LobbyProcedure>(procedureOwner);
        }
    }

    private bool IsLoginSuccess()
    {
        // 登录逻辑
        return true;
    }
}

public class LobbyProcedure : ImmoProcedureBase
{
    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        // 获取玩家信息
        if (procedureOwner.TryGetData("PlayerName", out string playerName))
        {
            Debug.Log($"欢迎 {playerName}!");
        }
        
        if (procedureOwner.TryGetData("PlayerId", out int playerId))
        {
            Debug.Log($"玩家ID: {playerId}");
        }
    }
}
```

### 4. 复杂流程示例

```csharp
/// <summary>
/// 更新资源流程。
/// </summary>
public class UpdateResourceProcedure : ImmoProcedureBase
{
    private bool m_UpdateComplete = false;
    private float m_UpdateProgress = 0f;

    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.Log("开始更新资源...");
        
        m_UpdateComplete = false;
        m_UpdateProgress = 0f;
        
        // 开始异步更新资源
        StartUpdateResources();
    }

    protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        // 显示更新进度
        Debug.Log($"更新进度: {m_UpdateProgress * 100:F1}%");
        
        if (m_UpdateComplete)
        {
            Debug.Log("资源更新完成");
            procedureOwner.SetData("ResourceVersion", "1.0.5");
            ChangeState<LoginProcedure>(procedureOwner);
        }
    }

    protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        // 清理更新资源相关数据
    }

    private void StartUpdateResources()
    {
        // 模拟异步更新
        // 实际项目中这里会调用资源更新模块
        m_UpdateComplete = true;
        m_UpdateProgress = 1f;
    }
}
```

## 典型游戏流程设计

### 单机游戏流程

```
启动流程 (Launch)
    ↓
主菜单流程 (MainMenu) ←─┐
    ↓                   │
游戏流程 (GamePlay) ────┤
    ↓                   │
暂停流程 (Pause) ───────┤
    ↓                   │
结算流程 (GameOver) ────┘
```

### 网络游戏流程

```
启动流程 (Launch)
    ↓
检查资源流程 (CheckResource)
    ↓
更新资源流程 (UpdateResource)
    ↓
选择服务器流程 (SelectServer)
    ↓
登录流程 (Login)
    ↓
大厅流程 (Lobby) ←──────┐
    ↓                   │
匹配流程 (Matching)      │
    ↓                   │
游戏流程 (GamePlay) ─────┤
    ↓                   │
结算流程 (GameOver) ─────┘
```

## 最佳实践

### 1. 流程职责划分
每个流程应该只负责一个特定的游戏阶段，保持职责单一：

```csharp
// ✅ 推荐：职责清晰
public class LoginProcedure : ImmoProcedureBase { }
public class LobbyProcedure : ImmoProcedureBase { }
public class GamePlayProcedure : ImmoProcedureBase { }

// ❌ 不推荐：职责混乱
public class MainProcedure : ImmoProcedureBase
{
    // 既处理登录，又处理大厅，又处理游戏
}
```

### 2. 数据管理
- 使用流程数据系统共享跨流程数据
- 在流程离开时清理临时数据
- 使用明确的数据键名

```csharp
protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
{
    // 设置数据
    procedureOwner.SetData("LevelId", 1);
    procedureOwner.SetData("Difficulty", "Normal");
}

protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
{
    // 清理临时数据
    procedureOwner.RemoveData("LevelId");
    procedureOwner.RemoveData("Difficulty");
}
```

### 3. 异步操作处理
在流程中处理异步操作时，应该正确管理状态：

```csharp
public class LoadSceneProcedure : ImmoProcedureBase
{
    private bool m_IsLoading = false;
    private bool m_LoadComplete = false;

    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        m_IsLoading = true;
        m_LoadComplete = false;
        
        // 开始异步加载场景
        LoadSceneAsync("GameScene", OnLoadComplete);
    }

    protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        if (m_LoadComplete)
        {
            ChangeState<GamePlayProcedure>(procedureOwner);
        }
    }

    private void OnLoadComplete()
    {
        m_IsLoading = false;
        m_LoadComplete = true;
    }
}
```

### 4. 错误处理
合理处理流程中的错误情况：

```csharp
public class LoginProcedure : ImmoProcedureBase
{
    protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        if (IsLoginFailed())
        {
            Debug.LogError("登录失败，返回主菜单");
            procedureOwner.SetData("LoginError", "网络连接失败");
            ChangeState<MainMenuProcedure>(procedureOwner);
        }
        else if (IsLoginSuccess())
        {
            ChangeState<LobbyProcedure>(procedureOwner);
        }
    }
}
```

## 与状态机的区别

虽然流程基于状态机实现，但它们有不同的应用场景：

| 特性 | 流程 (Procedure) | 状态机 (FSM) |
|------|-----------------|--------------|
| **作用域** | 全局，管理游戏生命周期 | 局部，管理单个对象行为 |
| **生命周期** | 贯穿整个游戏运行时 | 对象的生命周期内 |
| **实例数量** | 通常只有一个流程管理器 | 可以有多个状态机实例 |
| **典型用途** | 启动、登录、主菜单、游戏 | 角色行为、AI、动画控制 |
| **管理器** | ImmoProcedureManager | ImmoFsmManager |

## 注意事项

1. **单一流程管理器**：整个游戏只应该有一个流程管理器
2. **初始化顺序**：在使用流程前必须先调用 `Initialize` 方法
3. **流程切换**：优先在流程内部使用 `ChangeState` 切换流程
4. **数据清理**：及时清理不再使用的流程数据，避免内存泄漏
5. **异步操作**：正确处理流程中的异步操作，避免状态不一致

## 完整示例

参考 `ImmoProcedureExample.cs` 文件查看完整的使用示例。

---

**版本**: 1.0.0  
**创建日期**: 2025-01-30  
**参考**: [GameFramework Procedure Module](https://github.com/EllanJiang/GameFramework)
