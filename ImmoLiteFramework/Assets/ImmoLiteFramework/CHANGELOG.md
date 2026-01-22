# Changelog

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