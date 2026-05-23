# SF3 Web Port

Shadow Fight 3 — reverse-engineered Unity project ported to TypeScript + Babylon.js.

## Stack
- **Rendering**: Babylon.js 6 (ArcRotateCamera locked for 2.5D)
- **Language**: TypeScript (strict)
- **Tweening**: @tweenjs/tween.js (replaces DOTween)
- **UI**: HTML/CSS overlays (replaces NGUI)
- **Build**: Webpack 5 + ts-loader
- **Deploy**: GitHub Actions → GitHub Pages

## Folder Structure

```
src/
  scripts/
    core/          # Engine bootstrap, SceneManager, InputManager, AssetLoader
    battle/        # FightScene, BattleController, RoundController, MovesController
    ui/            # EnterPointScene, UIModulesController
    utils/         # Shared helpers
  assets/
    meshes/        # .glb character/weapon models (exported via UnityGLTF)
    textures/      # PNGs
    audio/         # .wav/.ogg
    animations/    # Animation metadata JSON (if needed outside glb)
  ui/
    index.html     # App shell
    screens/       # Per-screen HTML partials (if needed)
    components/    # Reusable UI component HTML
    styles/        # CSS files
  plugins/         # Third-party libs not on npm (copied to dist as-is)
```

## Scene Chain (mirrors SF3 source)

```
enterPoint.unity → EnterPointScene.ts
  └─ SceneInitializer → SceneManager.init()
  └─ UIModulesController → showScreen("main_menu")

fight.unity → FightScene.ts
  └─ BattleController (state machine)
       └─ RoundController (timer, hit reg)
       └─ MovesController × 2 (player + AI animation FSM)
```

## Dev Setup

```bash
npm install
npm run dev       # localhost:3000 with HMR
npm run build     # production build → dist/
npm run type-check
```

## Adding a Character Model

1. Export from Unity via UnityGLTF → `character_name.glb`
2. Drop into `src/assets/meshes/`
3. Load in code:
   ```ts
   const meshes = await assetLoader.loadMesh(scene, "character_name");
   ```

## GitHub Pages Deploy

Push to `main` → GitHub Actions builds → deploys `dist/` to Pages.
Enable Pages in repo Settings → Pages → Source: GitHub Actions.
