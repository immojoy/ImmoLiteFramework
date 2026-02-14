# Changelog

## [0.2.0] - 2026-02-15

### Added
- **Resource Manager - Scene Loading Support**
  - `LoadSceneAsync`: Asynchronous scene loading with Task-based API
  - `LoadSceneAsyncWithCallback`: Scene loading with callback-based API
  - Support for additive scene loading mode

### Changed
- **Resource Manager - Enhanced Callback System**
  - Refactored from single `OnAssetLoadSuccess` delegate to `OnAssetLoadCallback` struct
  - `OnAssetLoadCallback` now includes three callback types:
    - `SuccessCallback`: Called when asset loads successfully
    - `ProgressCallback`: Called during loading progress (reserved for future use)
    - `FailureCallback`: Called when asset loading fails
  - `LoadAssetAsyncWithCallback` method signature updated to use `OnAssetLoadCallback`
  - Improved error handling with detailed failure messages

- **Package Dependencies**
  - Removed `com.unity.textmeshpro` dependency
  - Updated Unity package registry from `packages.unity.cn` to `packages.unity.com`
  - Updated multiple Unity packages to latest versions

- **Project Configuration**
  - Upgraded Unity Editor from 2022.3.14f1c1 to 6000.0.20f1

### Removed
- `OnAssetLoadSuccess.cs` file (replaced by callback struct in `OnAssetLoadCallback.cs`)

## [0.1.0] - 2026-02-12

### Added
- **Scene Management Module**
  - `ImmoSceneManager`: Complete scene loading and unloading system
  - Asynchronous scene loading with progress tracking
  - Scene loading events (success, failure, update, start, unload)
  - Support for additive scene loading
  - Integration with resource management system
  - Editor menu: `GameObject/Immo Lite Framework/Manager/Scene Manager`

- **Procedure Management Module**
  - `ImmoProcedureManager`: Lifecycle-based procedure system
  - `ImmoProcedureBase`: Base class for custom procedures
  - Support for procedure initialization, entry, update, and exit callbacks
  - Procedure state transition system
  - Example procedures: Launch, MainMenu, GamePlay
  - Editor menu: `GameObject/Immo Lite Framework/Manager/Procedure Manager`

- **Finite State Machine Module**
  - `ImmoFsmManager`: Manager for multiple finite state machines
  - `ImmoFsm<T>`: Generic finite state machine implementation
  - `ImmoFsmState<T>`: Base class for FSM states
  - `IImmoFsm<T>`: FSM interface for state management
  - Support for state lifecycle callbacks (OnInit, OnEnter, OnUpdate, OnLeave, OnDestroy)
  - State transition system with type safety
  - Editor menu: `GameObject/Immo Lite Framework/Manager/Fsm Manager`

- **Modular Editor Scripts**
  - Separated editor creation scripts for better maintainability
  - `ImmoSceneCreator.cs`: Scene Manager editor utilities
  - `ImmoProcedureCreator.cs`: Procedure Manager editor utilities
  - `ImmoFsmCreator.cs`: FSM Manager editor utilities

### Changed
- **Complete Framework Creation** now includes all six managers:
  - Event Manager
  - Resource Manager
  - UI Manager
  - Scene Manager (new)
  - Procedure Manager (new)
  - Fsm Manager (new)

- **Documentation**
  - All module comments translated from Chinese to English
  - Comprehensive XML documentation for all public APIs
  - Improved code readability and maintainability

### Improved
- Better separation of concerns with modular editor scripts
- Enhanced framework initialization with all core systems
- Consistent coding standards across all modules


## [0.0.3] - 2026-01-22

### Added
- Enhanced editor menu structure with hierarchical organization
  - Moved "Immo Lite Framework" creation to `GameObject/Immo Lite Framework/Framework`
  - Added `GameObject/Immo Lite Framework/Manager` submenu for individual manager creation
  - Individual manager creation options: Event Manager, Resource Manager, UI Manager
- UI layer priority management system
  - Automatic Canvas sort order configuration for UI layers
  - `ImmoUiLayerConfig` ScriptableObject for customizable layer priorities
  - Five UI layers with predefined sort orders: Background (0), Normal (100), Popup (200), Top (300), System (400)
  - Automatic UI layer hierarchy creation with proper Canvas and GraphicRaycaster setup
  - Support for runtime layer priority adjustment

### Changed
- UI Manager now automatically creates complete UI layer structure when instantiated
- UI layers now use Canvas components with override sorting for proper rendering priority
- Editor scripts now create RectTransform components with full-screen anchors for UI layers

### Improved
- Better UI layer organization and management
- Enhanced framework initialization with automatic component setup
- Canvas Scaler automatically configured with 1920x1080 reference resolution


## [0.0.2] - 2026-01-22

### Added
- Support `GameObject` creation menu item to quickly create a ready framework object


## [0.0.1] - 2026-01-22

### Added
- Basic managers: event, UI, and resource