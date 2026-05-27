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

## Phase A — Fix Loading Screen (quick wins)

**A1. Remove the duplicate logo from `LoadScreen.ts`**
- Delete the `<img>` element (`logSF3.png`) from `LoadScreen.mount()`
- The Nekki logo is already handled by `GameLoad.ts` — that's the only one needed

**A2. Move loading screen HTML into `index.html` + CSS**
- Add the loading screen markup directly in `index.html` (hidden by default with CSS)
- Define all loading screen styles in `global.css` (background, spinner, tips position)
- `LoadScreen.mount()` → just shows the pre-built HTML element
- `LoadScreen.hide()` → fades out and removes it
- Use the `load_ring` sprite from the Common atlas as the spinner (authentic to the game)

**A3. Move tips to the bottom of the screen**
- CSS: tips container positioned at the bottom
- Keeps the background image clear and legible

**A4. Verify `GameLoad._showLogo()` / `_hideLogo()` timing**
- Nekki logo fades in → EnterPoint boots → LoadScreen hides → Nekki logo hides → render loop starts
- Ensure no visual flash or both screens visible at once

---

## Phase B — Build the EnterPoint UI (the big one)

**B1. Create the EnterPoint HTML template**
- `src/ui/screens/enter-point.html` (or embedded in index.html)
- Actual HTML structure with buttons, labels, panels matching the scene JSON hierarchy
- Use CSS to position elements, style text, handle responsive scaling

**B2. Atlas sprite system**
- `src/scripts/ui/AtlasManager.ts` — load and cache TexturePacker JSON atlases
- Map Unity GUIDs → actual atlas files (CommonJSON.json, FightJSON.json, etc.)
- Provide `getSpriteCss(spriteName)` → returns `background-image` + `background-position` + `width/height`
- Use CSS `background` on `<div>` elements rather than creating `<img>` for every sprite

**B3. Screen management**
- `EnterPointScene.ts` is rewritten to:
  - Show the pre-built HTML template
  - Inject dynamic data (player name, currency, version text, etc.)
  - Handle button click routing
- A simple screen/UI manager that can transition between loading → enter point → fight

**B4. NGUI coordinate → CSS conversion**
- Unity NGUI uses a 1280×720 virtual canvas with `pivot` points
- `UI Root` has scale `0.0027777778` (= 1/360) — this maps NGUI units to screen coords
- Convert `transform.position` + `uiWidget.width/height` + `pivot` to proper CSS positioning

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
| `src/ui/index.html` | A, B | Add loading screen HTML + enter point HTML |
| `src/ui/styles/global.css` | A, B | Loading screen styles, enter point styles, responsive scaling |
| `src/scripts/LoadScreen.ts` | A | Refactor to use HTML/CSS, remove duplicate logo |
| `src/scripts/GameLoad.ts` | A | Verify timing (no changes expected) |
| `src/scripts/core/main.ts` | A, B | May need minor adjustments |
| `src/scripts/ui/EnterPointScene.ts` | B | Complete rewrite — show HTML template, inject data |
| `src/scripts/ui/AtlasManager.ts` | B | **New** — atlas loading + sprite CSS lookup |
| `src/scripts/ui/ScreenManager.ts` | B | **New** — simple screen show/hide manager |
| `src/assets/textures/ui/atlases/*.png` | B | Already exist |
| `src/assets/textures/ui/nativeui/atlases/*JSON.json` | B | Already exist |
