# 场景管理模块 (Scene)

## 概述

ImmoLiteFramework 的场景管理模块基于 GameFramework 的设计思想，提供了一个统一的场景加载和卸载管理系统。该模块封装了 Unity 的 SceneManager，提供了更友好的 API、事件系统和状态管理。

## 核心特性

- **异步加载/卸载**：所有场景操作都是异步的，不会阻塞主线程
- **状态管理**：跟踪场景的加载、已加载、卸载状态
- **事件系统**：提供场景加载/卸载的成功、失败、更新事件
- **进度跟踪**：实时获取场景加载进度
- **多场景支持**：支持单场景和附加场景模式
- **错误处理**：完善的错误检查和异常处理

## 核心类

### `ImmoSceneManager`

场景管理器，负责管理所有场景的加载和卸载。

**单例访问**：
```csharp
ImmoSceneManager.Instance
```

**核心事件**：
- `LoadSceneSuccess` - 加载场景成功事件 `Action<string sceneName, float duration>`
- `LoadSceneFailure` - 加载场景失败事件 `Action<string sceneName, string errorMessage>`
- `LoadSceneUpdate` - 加载场景更新事件 `Action<string sceneName, float progress>`
- `UnloadSceneSuccess` - 卸载场景成功事件 `Action<string sceneName>`
- `UnloadSceneFailure` - 卸载场景失败事件 `Action<string sceneName, string errorMessage>`

**核心方法**：

#### 场景状态查询
- `bool IsSceneLoaded(string sceneName)` - 检查场景是否已加载
- `bool IsSceneLoading(string sceneName)` - 检查场景是否正在加载
- `bool IsSceneUnloading(string sceneName)` - 检查场景是否正在卸载
- `string[] GetLoadedSceneNames()` - 获取所有已加载的场景名称
- `string[] GetLoadingSceneNames()` - 获取所有正在加载的场景名称
- `string[] GetUnloadingSceneNames()` - 获取所有正在卸载的场景名称

#### 场景加载
- `void LoadScene(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)` - 加载场景
- `void LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single)` - 异步加载场景

#### 场景卸载
- `void UnloadScene(string sceneName)` - 卸载场景
- `void UnloadSceneAsync(string sceneName)` - 异步卸载场景

#### 活动场景管理
- `Scene GetActiveScene()` - 获取当前活动场景
- `bool SetActiveScene(string sceneName)` - 设置活动场景

## 使用示例

### 1. 基本场景加载

```csharp
using Immojoy.LiteFramework.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private void Start()
    {
        // 加载单个场景（替换当前场景）
        ImmoSceneManager.Instance.LoadScene("Level1", LoadSceneMode.Single);

        // 加载附加场景（不替换当前场景）
        ImmoSceneManager.Instance.LoadScene("UIScene", LoadSceneMode.Additive);
    }
}
```

### 2. 带事件监听的场景加载

```csharp
using Immojoy.LiteFramework.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        // 订阅事件
        ImmoSceneManager.Instance.LoadSceneSuccess += OnLoadSceneSuccess;
        ImmoSceneManager.Instance.LoadSceneFailure += OnLoadSceneFailure;
        ImmoSceneManager.Instance.LoadSceneUpdate += OnLoadSceneUpdate;

        // 加载场景
        ImmoSceneManager.Instance.LoadSceneAsync("MainMenu");
    }

    private void OnDestroy()
    {
        // 取消订阅
        if (ImmoSceneManager.Instance != null)
        {
            ImmoSceneManager.Instance.LoadSceneSuccess -= OnLoadSceneSuccess;
            ImmoSceneManager.Instance.LoadSceneFailure -= OnLoadSceneFailure;
            ImmoSceneManager.Instance.LoadSceneUpdate -= OnLoadSceneUpdate;
        }
    }

    private void OnLoadSceneSuccess(string sceneName, float duration)
    {
        Debug.Log($"场景 '{sceneName}' 加载成功，耗时 {duration:F2} 秒");
        // 场景加载完成后的逻辑
    }

    private void OnLoadSceneFailure(string sceneName, string errorMessage)
    {
        Debug.LogError($"场景 '{sceneName}' 加载失败: {errorMessage}");
        // 处理加载失败
    }

    private void OnLoadSceneUpdate(string sceneName, float progress)
    {
        Debug.Log($"场景 '{sceneName}' 加载进度: {progress * 100f:F1}%");
        // 更新加载进度条
    }
}
```

### 3. 带加载界面的场景切换

```csharp
using Immojoy.LiteFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private GameObject m_LoadingPanel;
    [SerializeField] private Slider m_ProgressBar;
    [SerializeField] private Text m_LoadingText;

    public void LoadGameScene(string sceneName)
    {
        // 显示加载界面
        m_LoadingPanel.SetActive(true);

        // 订阅进度事件
        ImmoSceneManager.Instance.LoadSceneUpdate += OnLoadSceneUpdate;
        ImmoSceneManager.Instance.LoadSceneSuccess += OnLoadSceneSuccess;

        // 开始加载场景
        ImmoSceneManager.Instance.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    private void OnLoadSceneUpdate(string sceneName, float progress)
    {
        // 更新进度条
        m_ProgressBar.value = progress;
        m_LoadingText.text = $"Loading {sceneName}... {progress * 100f:F0}%";
    }

    private void OnLoadSceneSuccess(string sceneName, float duration)
    {
        // 隐藏加载界面
        m_LoadingPanel.SetActive(false);

        // 取消订阅
        ImmoSceneManager.Instance.LoadSceneUpdate -= OnLoadSceneUpdate;
        ImmoSceneManager.Instance.LoadSceneSuccess -= OnLoadSceneSuccess;

        Debug.Log($"场景 '{sceneName}' 加载完成");
    }
}
```

### 4. 多场景管理

```csharp
using Immojoy.LiteFramework.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiSceneManager : MonoBehaviour
{
    private void Start()
    {
        // 加载主场景
        ImmoSceneManager.Instance.LoadSceneAsync("GameWorld", LoadSceneMode.Single);

        // 加载附加场景（UI、音频等）
        ImmoSceneManager.Instance.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
        ImmoSceneManager.Instance.LoadSceneAsync("AudioScene", LoadSceneMode.Additive);
    }

    public void LoadLevel(int levelIndex)
    {
        // 卸载旧关卡
        string[] loadedScenes = ImmoSceneManager.Instance.GetLoadedSceneNames();
        foreach (string sceneName in loadedScenes)
        {
            if (sceneName.StartsWith("Level"))
            {
                ImmoSceneManager.Instance.UnloadSceneAsync(sceneName);
            }
        }

        // 加载新关卡
        ImmoSceneManager.Instance.LoadSceneAsync($"Level{levelIndex}", LoadSceneMode.Additive);
    }

    public void SetActiveGameScene(string sceneName)
    {
        // 设置活动场景
        if (ImmoSceneManager.Instance.SetActiveScene(sceneName))
        {
            Debug.Log($"活动场景已设置为: {sceneName}");
        }
        else
        {
            Debug.LogError($"无法设置活动场景: {sceneName}");
        }
    }
}
```

### 5. 场景预加载系统

```csharp
using Immojoy.LiteFramework.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePreloader : MonoBehaviour
{
    [SerializeField] private List<string> m_ScenesToPreload = new List<string>();
    private int m_LoadedCount = 0;

    private void Start()
    {
        PreloadScenes();
    }

    private void PreloadScenes()
    {
        if (m_ScenesToPreload.Count == 0)
        {
            Debug.LogWarning("No scenes to preload.");
            return;
        }

        ImmoSceneManager.Instance.LoadSceneSuccess += OnScenePreloaded;

        foreach (string sceneName in m_ScenesToPreload)
        {
            ImmoSceneManager.Instance.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    private void OnScenePreloaded(string sceneName, float duration)
    {
        m_LoadedCount++;
        Debug.Log($"预加载场景 '{sceneName}' 完成 ({m_LoadedCount}/{m_ScenesToPreload.Count})");

        if (m_LoadedCount >= m_ScenesToPreload.Count)
        {
            ImmoSceneManager.Instance.LoadSceneSuccess -= OnScenePreloaded;
            Debug.Log("所有场景预加载完成！");
            OnAllScenesPreloaded();
        }
    }

    private void OnAllScenesPreloaded()
    {
        // 所有场景预加载完成后的逻辑
    }
}
```

### 6. 场景卸载

```csharp
using Immojoy.LiteFramework.Runtime;
using UnityEngine;

public class SceneUnloader : MonoBehaviour
{
    public void UnloadScene(string sceneName)
    {
        if (!ImmoSceneManager.Instance.IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' is not loaded.");
            return;
        }

        // 订阅卸载事件
        ImmoSceneManager.Instance.UnloadSceneSuccess += OnUnloadSceneSuccess;
        ImmoSceneManager.Instance.UnloadSceneFailure += OnUnloadSceneFailure;

        // 卸载场景
        ImmoSceneManager.Instance.UnloadSceneAsync(sceneName);
    }

    private void OnUnloadSceneSuccess(string sceneName)
    {
        Debug.Log($"Scene '{sceneName}' unloaded successfully.");

        // 取消订阅
        ImmoSceneManager.Instance.UnloadSceneSuccess -= OnUnloadSceneSuccess;
        ImmoSceneManager.Instance.UnloadSceneFailure -= OnUnloadSceneFailure;
    }

    private void OnUnloadSceneFailure(string sceneName, string errorMessage)
    {
        Debug.LogError($"Failed to unload scene '{sceneName}': {errorMessage}");

        // 取消订阅
        ImmoSceneManager.Instance.UnloadSceneSuccess -= OnUnloadSceneSuccess;
        ImmoSceneManager.Instance.UnloadSceneFailure -= OnUnloadSceneFailure;
    }
}
```

### 7. 与流程模块结合使用

```csharp
using Immojoy.LiteFramework.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单流程 - 加载主菜单场景。
/// </summary>
public class MainMenuProcedure : ImmoProcedureBase
{
    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        // 订阅场景加载事件
        ImmoSceneManager.Instance.LoadSceneSuccess += OnLoadSceneSuccess;

        // 加载主菜单场景
        ImmoSceneManager.Instance.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

    protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);

        // 取消订阅
        ImmoSceneManager.Instance.LoadSceneSuccess -= OnLoadSceneSuccess;
    }

    private void OnLoadSceneSuccess(string sceneName, float duration)
    {
        Debug.Log($"主菜单场景加载完成");
        // 可以在这里初始化主菜单 UI
    }
}

/// <summary>
/// 游戏流程 - 加载游戏场景。
/// </summary>
public class GameProcedure : ImmoProcedureBase
{
    protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        // 订阅场景加载事件
        ImmoSceneManager.Instance.LoadSceneSuccess += OnLoadSceneSuccess;
        ImmoSceneManager.Instance.LoadSceneUpdate += OnLoadSceneUpdate;

        // 加载游戏场景
        int levelId = procedureOwner.GetData<int>("LevelId");
        ImmoSceneManager.Instance.LoadSceneAsync($"Level{levelId}", LoadSceneMode.Single);
    }

    protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);

        // 取消订阅
        ImmoSceneManager.Instance.LoadSceneSuccess -= OnLoadSceneSuccess;
        ImmoSceneManager.Instance.LoadSceneUpdate -= OnLoadSceneUpdate;
    }

    private void OnLoadSceneSuccess(string sceneName, float duration)
    {
        Debug.Log($"游戏场景 '{sceneName}' 加载完成");
        // 初始化游戏逻辑
    }

    private void OnLoadSceneUpdate(string sceneName, float progress)
    {
        // 更新加载进度界面
        Debug.Log($"加载进度: {progress * 100f:F1}%");
    }
}
```

## LoadSceneMode 说明

Unity 的 `LoadSceneMode` 有两种模式：

- **LoadSceneMode.Single**：单场景模式，加载新场景时会卸载所有已加载的场景（默认）
- **LoadSceneMode.Additive**：附加模式，加载新场景时不会卸载已有场景，支持多场景共存

### 使用场景

**Single 模式适用于**：
- 场景之间完全独立
- 关卡切换
- 主菜单到游戏的切换

**Additive 模式适用于**：
- UI 场景与游戏场景分离
- 音频管理场景
- 多人游戏中的子场景
- 动态加载/卸载的游戏区域

## 最佳实践

### 1. 场景命名规范

```csharp
// ✅ 推荐：使用清晰的命名
"MainMenu"
"Level1"
"UIScene"
"AudioScene"

// ❌ 不推荐：模糊的命名
"Scene1"
"Test"
"Temp"
```

### 2. 事件订阅管理

```csharp
// ✅ 推荐：在 Start 中订阅，在 OnDestroy 中取消订阅
private void Start()
{
    ImmoSceneManager.Instance.LoadSceneSuccess += OnLoadSceneSuccess;
}

private void OnDestroy()
{
    if (ImmoSceneManager.Instance != null)
    {
        ImmoSceneManager.Instance.LoadSceneSuccess -= OnLoadSceneSuccess;
    }
}

// ❌ 不推荐：不取消订阅，可能导致内存泄漏
```

### 3. 状态检查

```csharp
// ✅ 推荐：加载前检查状态
if (!ImmoSceneManager.Instance.IsSceneLoaded("Level1") &&
    !ImmoSceneManager.Instance.IsSceneLoading("Level1"))
{
    ImmoSceneManager.Instance.LoadScene("Level1");
}

// ❌ 不推荐：直接加载，可能导致重复加载
ImmoSceneManager.Instance.LoadScene("Level1");
```

### 4. 错误处理

```csharp
// ✅ 推荐：订阅失败事件并处理
ImmoSceneManager.Instance.LoadSceneFailure += (sceneName, errorMessage) =>
{
    Debug.LogError($"加载失败: {errorMessage}");
    // 显示错误提示给玩家
    // 重试或返回主菜单
};

// ❌ 不推荐：不处理错误
```

### 5. 活动场景管理

```csharp
// ✅ 推荐：在多场景模式下设置活动场景
ImmoSceneManager.Instance.LoadSceneAsync("GameWorld", LoadSceneMode.Additive);
ImmoSceneManager.Instance.LoadSceneSuccess += (sceneName, duration) =>
{
    if (sceneName == "GameWorld")
    {
        ImmoSceneManager.Instance.SetActiveScene("GameWorld");
    }
};

// ❌ 不推荐：不设置活动场景，可能导致光照和物理问题
```

## 注意事项

1. **单例模式**：`ImmoSceneManager` 使用单例模式，确保场景中只有一个实例
2. **异步操作**：所有场景加载/卸载都是异步的，需要通过事件监听结果
3. **事件清理**：记得在对象销毁时取消事件订阅，避免内存泄漏
4. **场景名称**：场景名称必须与 Build Settings 中的场景名称完全一致
5. **活动场景**：在 Additive 模式下，建议手动设置活动场景

## 与 GameFramework 的区别

| 特性 | GameFramework | ImmoLiteFramework |
|------|---------------|-------------------|
| **依赖** | 依赖资源管理模块 | 直接使用 Unity SceneManager |
| **回调方式** | 回调函数集 | C# 事件 |
| **API 复杂度** | 较复杂（更多配置选项） | 简化（常用功能） |
| **优先级系统** | 支持 | 不支持 |
| **资源管理** | 集成资源加载系统 | 使用 Unity 内置系统 |

## 完整示例

参考 `ImmoSceneExample.cs` 文件查看完整的使用示例。

---

**版本**: 1.0.0  
**创建日期**: 2025-01-30  
**参考**: [GameFramework Scene Module](https://github.com/EllanJiang/GameFramework)
