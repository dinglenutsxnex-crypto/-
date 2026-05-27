# SF3 Web Port — Conversion Plan

## Current State

The project is a Unity (Shadow Fight 3) → BabylonJS web port. Most of the C# game logic has been ported to TypeScript files. However, the UI layer is almost nonexistent:

1. **Loading screen** — DOM elements created inline with JS `Object.assign(style)`, logo duplicated (appears in both `LoadScreen` and `GameLoad`), tips centered in middle instead of bottom
2. **EnterPoint scene UI** — `EnterPointScene.mount()` creates empty `<div>` elements from the scene JSON hierarchy, ignoring all `uiLabel`, `uiSprite`, `uiWidget`, `uiPanel` component data → **black screen with no visible UI after load**

---

## Approach: HTML/CSS-first UI

Instead of generating DOM elements programmatically with inline styles (Unity NGUI approach), we use the freedom of the web platform:

- **HTML templates** for each screen (loading screen, enter point, etc.)
- **CSS** for layout, positioning, animations, theming
- **JS/TypeScript** only for behavior (show/hide, load data, handle input)
- Dynamic content (tips, user data) injected into pre-built HTML shells

This separates concerns properly and makes the UI easy to iterate on.

---

## Phase A — Fix Loading Screen ✓ (DONE)

**A1.** Remove duplicate Nekki logo from `LoadScreen.ts`
**A2.** Move loading screen HTML into `index.html` + CSS refactor into `global.css`
**A3.** Move tips to bottom of screen
**A4.** Verify `GameLoad._showLogo()` / `_hideLogo()` timing

---

## Phase B — Build the EnterPoint UI (Currency Bar + Home Menu)

The EnterPoint screen consists of two Unity prefabs ported to HTML/CSS/JS:

1. **Currency Bar** (`Currency.prefab`) — top bar with player info, XP, currencies
2. **Home Menu** (`HomeMenu.prefab`) — slide-out navigation panel

### B1. Atlas Sprite System

**File: `src/scripts/ui/AtlasManager.ts` (NEW)**

Loads TexturePacker JSON atlas files + hardcoded DojoMenu sprite data at runtime:

- `fetch()` + parse CommonJSON.json, CurrencyJSON.json
- Hardcoded DojoMenu sprite rects extracted from Unity `.asset` files
- Provides `getSpriteStyle(name: string): CSSStyleDeclaration | null`
- Generates `background-image`, `background-position`, `width`, `height` from frame data

**Sprite-to-Atlas mapping:**

| Sprite Name | Atlas PNG | Atlas JSON | Source |
|---|---|---|---|
| `menu_icon` | `DojoMenu.png` (512×512) | — (hardcoded) | Unity .asset files |
| `dojo_icon` | `DojoMenu.png` | — | (1, 137, 94, 58) |
| `map_icon` | `DojoMenu.png` | — | (5, 223, 142, 111) |
| `shop_icon` | `DojoMenu.png` | — | (308, 226, 141, 99) |
| `inventory_icon` | `DojoMenu.png` | — | (255, 55, 134, 134) |
| `chat.png` | `Currency.png` | `CurrencyJSON.json` | (169, 211, 116, 129) |
| `cross.png` | `Currency.png` | `CurrencyJSON.json` | (1, 1, 101, 67) |
| `bonus.png` | `Common.png` | `CommonJSON.json` | (412, 1, 51, 52) |
| `coin.png` | `Common.png` | `CommonJSON.json` | (217, 1, 60, 72) |
| `circle.png` | `Common.png` | `CommonJSON.json` | (278, 1, 90, 90) |
| `progress_empty.png` | `Currency.png` | `CurrencyJSON.json` | (99, 1, 52, 52) |
| `progress_full.png` | `Currency.png` | `CurrencyJSON.json` | (1, 104, 460, 19) |

### B2. Currency Bar

**HTML: `src/ui/screens/currency-bar.html` (NEW)**
**CSS: `src/ui/styles/screens/currency-bar.css` (NEW)**
**TS: integrated into `EnterPointScene.ts`**

Structure (mirrors Currency.prefab YAML):

```
#currency-bar
├── #currency-player-info
│   ├── #currency-name          ← "PLAYER" (white, font-size ~18px)
│   ├── #currency-level         ← "2" (gold #877832, font-size ~18px)
│   ├── #currency-progress      ← slider 87%
│   │   ├── .progress-bg        ← #737373
│   │   └── .progress-fill      ← #D5D5D5 (width: 87%)
│   └── #currency-chat          ← chat.png sprite
└── #currency-coins
    ├── .currency-item.bonus
    │   ├── .currency-icon      ← bonus.png sprite
    │   ├── .currency-btn-add   ← cross.png sprite
    │   └── .currency-value     ← "400"
    ├── .currency-item.coin
    │   ├── .currency-icon      ← coin.png sprite
    │   ├── .currency-btn-add   ← cross.png sprite
    │   └── .currency-value     ← "269"
    └── .currency-item.shadow
        ├── .currency-icon      ← (no sprite, CSS shape or hidden)
        └── .currency-value     ← "0"
```

**Colors / dimensions from prefab:**
- Bar height: 138px, background: `#0B0B0C`
- Full width, anchored top
- PlayerInfo: 637×138, left side
- Coins: 900×138, right side (anchored right)
- Progress: 460×16, bg `#737373`, fill `#D5D5D5`
- Font "PLAYER": guid `7cf20e`, white, size 50 (scaled)
- Font level: guid `dc1789`, gold `#877832`

### B3. Home Menu

**HTML: `src/ui/screens/home-menu.html` (NEW)**
**CSS: `src/ui/styles/screens/home-menu.css` (NEW)**
**TS: integrated into `EnterPointScene.ts`**

Structure (mirrors HomeMenu.prefab YAML):

```
#home-menu
├── #home-button                ← toggles menu open/close
│   └── .home-icon              ← menu_icon sprite (94×58, centered in 190×138)
├── #home-backplane             ← dark overlay #1C1C1C alpha 0.32, click to close
├── #home-screen-background     ← full-screen click-to-close
└── #home-sub-menu              ← slide-out panel (842px wide)
    ├── #home-buttons-holder
    │   ├── .menu-btn[data-menu="map"]
    │   │   ├── .menu-btn-bg    ← solid #141414, 152×100
    │   │   ├── .menu-btn-icon  ← map_icon (60×60 centered)
    │   │   └── .menu-btn-badge ← red circle #E64421 + "20" text
    │   ├── .menu-btn[data-menu="shop"]
    │   │   ├── .menu-btn-bg
    │   │   └── .menu-btn-icon  ← shop_icon
    │   ├── .menu-btn[data-menu="dojo"]
    │   │   ├── .menu-btn-bg
    │   │   └── .menu-btn-icon  ← dojo_icon
    │   ├── .menu-btn[data-menu="inventory"]
    │   │   ├── .menu-btn-bg
    │   │   └── .menu-btn-icon  ← inventory_icon
    │   └── .menu-btn[data-menu="boosterpacks"]
    │       ├── .menu-btn-bg
    │       └── .menu-btn-icon  ← (no icon, text only)
```

**Behaviors (from `SlideMenu.cs`):**
- `home-button` click → `Open()` → slides sub-menu in from left (0.2s), fades backplane to alpha 0.5
- `screen-background` click → `CloseWithoutCooldown()` → slides out, resets
- Sub-menu buttons → `OpenMenu("map")` etc. → triggers module navigation + closes menu
- Menu initially hidden (offscreen left by panel width)
- Button selected alpha: 1.0, unselected: 0.57 (CanvasGroup)

**Button layout (from `sub_menu_buttons`):**
- Vertical list, each button: `backplane_4` style background or solid `#141414`, 152×100
- 160×100 click area (BoxCollider in prefab)
- Icon 60×60 centered within button
- Map has badge (red circle 30×30 + "20" text)
- `settings_placeholder` and `boosterpacks` buttons (settings inactive)

### B4. File Structure

```
src/ui/
├── index.html                          ← canvas, load screen, enter-point container
├── screens/
│   ├── currency-bar.html               ← currency bar template (fetched at runtime)
│   └── home-menu.html                  ← home menu template (fetched at runtime)
└── styles/
    ├── global.css                      ← loading screen + base enter-point styles
    ├── screens/
    │   ├── currency-bar.css            ← currency bar layout & colors
    │   └── home-menu.css               ← home menu layout, slide animation
src/scripts/
├── ui/
│   ├── AtlasManager.ts (NEW)           ← loads JSON atlases + DojoMenu hardcoded data
│   └── EnterPointScene.ts (REWRITE)    ← mounts/dismounts UI, injects data, handles clicks
```

### B5. Data Flow

1. `EnterPoint.init()` loads: InternalSettings → UserData → Scene JSON
2. `EnterPointScene.mount()` called from `_loadEnterPointScene()`
3. `mount()`:
   a. Creates Babylon camera from scene JSON
   b. Fetches `currency-bar.html` and `home-menu.html` templates
   c. Inserts into DOM `#enter-point-ui` container
   d. Loads atlas JSON files (Common, Currency)
   e. Injects player data (name, level, XP, currencies) from `UserDataController.player`
   f. Wires click handlers (home button toggle, sub-menu navigation)
   g. Hides loading screen
4. UI is fully interactive at this point

### B6. Animation Details (from C#)

| Behavior | C# Source | CSS Equivalent |
|---|---|---|
| Slide menu in | `_menuPanelRect.DOLocalMoveX(0, 0.2s)` | `transform: translateX(0)` with `transition: 0.2s` |
| Slide menu out | `DOLocalMoveX(-panelWidth, 0.2s)` | `transform: translateX(-100%)` with `transition: 0.2s` |
| Backplane fade in | `DOFade(0.5, 0.2s)` | `opacity: 0.5` with `transition: 0.2s` |
| Backplane fade out | `DOFade(0, 0.2s)` | `opacity: 0` with `transition: 0.2s` |
| Button select alpha | CanvasGroup alpha → 1.0 | `opacity: 1` |
| Button deselect alpha | CanvasGroup alpha → 0.57 | `opacity: 0.57` |
| Currency animation | DOTween.To (1s) | CSS `transition` on textContent change |

---

## Phase C — Polish & Integration

**C1. Responsive scaling**
- Scale the 1280×720 virtual canvas to any viewport size while maintaining aspect ratio
- Handle both portrait and landscape orientations

**C2. Stub files → real implementations**
- `UIModulesController.ts` — actually switch between UI screens
- `InputManager.ts` — handle keyboard/touch input for UI
- `AssetLoader.ts` — manage loading of GLBs, textures, animations

**C3. Fight scene transition**
- Wire up the "Fight" button from EnterPoint UI → `FightScene` loading → 3D battle
- Show loading progress during GLB asset loading

---

## File Change Summary

| File | Phase | Action |
|------|-------|--------|
| `src/ui/index.html` | A, B | Add loading screen HTML + enter-point container |
| `src/ui/styles/global.css` | A, B | Loading screen styles, enter point base styles |
| `src/scripts/LoadScreen.ts` | A | Refactor to use HTML/CSS, remove duplicate logo |
| `src/scripts/EnterPoint.ts` | B | Minor — ensure scene mounts before hide |
| `src/scripts/ui/EnterPointScene.ts` | B | Complete rewrite — show HTML templates, inject data |
| `src/scripts/ui/AtlasManager.ts` | B | **New** — atlas loading + sprite CSS lookup |
| `src/ui/screens/currency-bar.html` | B | **New** — currency bar HTML template |
| `src/ui/screens/home-menu.html` | B | **New** — home menu HTML template |
| `src/ui/styles/screens/currency-bar.css` | B | **New** — currency bar specific styles |
| `src/ui/styles/screens/home-menu.css` | B | **New** — home menu specific styles |
| `src/assets/textures/ui/nativeui/atlases/*JSON.json` | B | Already exist |
| `src/assets/textures/ui/nativeui/atlases/*.png` | B | Already exist |
