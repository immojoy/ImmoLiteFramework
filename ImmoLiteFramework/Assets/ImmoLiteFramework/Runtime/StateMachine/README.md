# 状态机模块 (Finite State Machine)

## 概述

ImmoLiteFramework 的状态机模块基于 GameFramework 的设计思想，提供了一个强大且灵活的有限状态机(FSM)实现。

## 核心特性

- **类型安全**: 使用泛型确保状态机类型安全
- **生命周期管理**: 完整的状态生命周期回调（OnInit, OnEnter, OnUpdate, OnLeave, OnDestroy）
- **集中管理**: 通过 ImmoFsmManager 统一管理所有状态机
- **高性能**: 自动轮询更新，支持逻辑时间和真实时间

## 核心类

### 1. `ImmoFsmState<T>`
状态基类，所有自定义状态都需要继承此类。

**生命周期方法**:
- `OnInit(IImmoFsm<T> fsm)` - 状态初始化时调用一次
- `OnEnter(IImmoFsm<T> fsm)` - 每次进入状态时调用
- `OnUpdate(IImmoFsm<T> fsm, float elapseSeconds, float realElapseSeconds)` - 状态运行时每帧调用
- `OnLeave(IImmoFsm<T> fsm, bool isShutdown)` - 每次离开状态时调用
- `OnDestroy(IImmoFsm<T> fsm)` - 状态销毁时调用一次

**状态切换**:
- `ChangeState<TState>(IImmoFsm<T> fsm)` - 切换到指定类型的状态
- `ChangeState(IImmoFsm<T> fsm, Type stateType)` - 切换到指定类型的状态

### 2. `IImmoFsm<T>`
有限状态机接口，提供状态机的基本操作。

**属性**:
- `Name` - 状态机名称
- `Owner` - 状态机持有者
- `FsmStateCount` - 状态机中状态的数量
- `IsRunning` - 状态机是否正在运行
- `IsDestroyed` - 状态机是否已被销毁
- `CurrentState` - 当前活跃的状态
- `CurrentStateTime` - 当前状态持续时间

**方法**:
- `Start<TState>()` - 启动状态机
- `HasState<TState>()` - 检查是否存在指定状态
- `GetState<TState>()` - 获取指定状态
- `GetAllStates()` - 获取所有状态

### 3. `ImmoFsmManager`
状态机管理器，负责创建、管理和销毁所有状态机。

**单例访问**:
```csharp
ImmoFsmManager.Instance
```

**核心方法**:
- `CreateFsm<T>(T owner, params ImmoFsmState<T>[] states)` - 创建状态机
- `GetFsm<T>(string name = "")` - 获取状态机
- `HasFsm<T>(string name = "")` - 检查状态机是否存在
- `DestroyFsm<T>(string name = "")` - 销毁状态机

## 使用示例

### 1. 定义状态类

```csharp
public class IdleState : ImmoFsmState<Player>
{
    protected internal override void OnEnter(IImmoFsm<Player> fsm)
    {
        base.OnEnter(fsm);
        Debug.Log("进入空闲状态");
    }

    protected internal override void OnUpdate(IImmoFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
        
        // 检测输入，切换到移动状态
        if (Input.GetKey(KeyCode.W))
        {
            ChangeState<MoveState>(fsm);
        }
    }

    protected internal override void OnLeave(IImmoFsm<Player> fsm, bool isShutdown)
    {
        base.OnLeave(fsm, isShutdown);
        Debug.Log("离开空闲状态");
    }
}

public class MoveState : ImmoFsmState<Player>
{
    protected internal override void OnEnter(IImmoFsm<Player> fsm)
    {
        base.OnEnter(fsm);
        Debug.Log("进入移动状态");
    }

    protected internal override void OnUpdate(IImmoFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
        
        // 移动逻辑
        fsm.Owner.transform.Translate(Vector3.forward * 5f * elapseSeconds);
        
        // 检测输入，切换回空闲状态
        if (!Input.GetKey(KeyCode.W))
        {
            ChangeState<IdleState>(fsm);
        }
    }

    protected internal override void OnLeave(IImmoFsm<Player> fsm, bool isShutdown)
    {
        base.OnLeave(fsm, isShutdown);
        Debug.Log("离开移动状态");
    }
}
```

### 2. 创建和启动状态机

```csharp
public class Player : MonoBehaviour
{
    private IImmoFsm<Player> m_Fsm;

    private void Start()
    {
        // 创建状态实例
        IdleState idleState = new IdleState();
        MoveState moveState = new MoveState();
        AttackState attackState = new AttackState();

        // 创建状态机
        m_Fsm = ImmoFsmManager.Instance.CreateFsm(this, idleState, moveState, attackState);

        // 启动状态机，从空闲状态开始
        m_Fsm.Start<IdleState>();
    }

    private void OnDestroy()
    {
        // 销毁状态机
        if (ImmoFsmManager.Instance != null && m_Fsm != null)
        {
            ImmoFsmManager.Instance.DestroyFsm(m_Fsm);
        }
    }
}
```

### 3. 多个同类型状态机

```csharp
// 为同一类型的不同实例创建多个状态机，使用名称区分
IImmoFsm<Enemy> enemyFsm1 = ImmoFsmManager.Instance.CreateFsm("Enemy1", enemy1, states);
IImmoFsm<Enemy> enemyFsm2 = ImmoFsmManager.Instance.CreateFsm("Enemy2", enemy2, states);

// 获取指定名称的状态机
IImmoFsm<Enemy> fsm = ImmoFsmManager.Instance.GetFsm<Enemy>("Enemy1");
```

## 最佳实践

1. **状态粒度**: 保持状态职责单一，避免单个状态过于复杂
2. **状态切换**: 优先在状态内部使用 `ChangeState` 切换，保持逻辑清晰
3. **资源管理**: 在 `OnEnter` 中初始化资源，在 `OnLeave` 中清理资源
4. **状态数据**: 将状态相关的数据保存在状态类的字段中
5. **持有者访问**: 通过 `fsm.Owner` 访问状态机持有者的属性和方法
6. **时间参数**: `elapseSeconds` 用于游戏逻辑，`realElapseSeconds` 用于 UI 等不受时间缩放影响的逻辑

## 与 GameFramework 的区别

1. **命名规范**: 遵循 ImmoLiteFramework 的命名规范（Immo 前缀，m_ 字段前缀）
2. **简化设计**: 移除了引用池等复杂特性，降低学习成本
3. **Unity 集成**: 直接继承 MonoBehaviour，更符合 Unity 使用习惯
4. **数据管理**: 暂未实现状态机数据管理功能，可在后续版本中添加

## 注意事项

1. 确保场景中存在 `ImmoFsmManager` 组件
2. 状态机启动前必须先调用 `CreateFsm` 创建
3. 不要在状态的 `OnInit` 中切换状态
4. 避免在 `OnUpdate` 中频繁创建新对象，注意性能
5. 及时销毁不再使用的状态机，避免内存泄漏

## 完整示例

参考 `ImmoFsmExample.cs` 文件查看完整的使用示例。

---

**版本**: 1.0.0  
**创建日期**: 2026-01-30  
**参考**: [GameFramework FSM Module](https://github.com/EllanJiang/GameFramework)
