---
name: SF3 web viewport/aspect-ratio scaling
description: How the 2D UI overlay and 3D canvas are sized to fit arbitrary browser/webview aspect ratios without cropping.
---

The 2D HUD lives in a fixed 1280x720 `#ui-viewport` div, scaled via inline JS in `src/ui/index.html`.
It must use **contain** scaling (`Math.min(winW/W, winH/H)`), not **cover** (`Math.max`) — cover crops
the HUD's edges whenever the real aspect ratio differs from 16:9, which is exactly what happens in
WebView fullscreen wrappers and any device narrower/wider than 16:9.

**Why:** a WebView user reported the top bar overlapping the game and the sides "flowing out" —
both were symptoms of the old cover-mode crop, not of Babylon's 3D camera (which naturally adapts
FOV to any aspect and doesn't need letterboxing).

**How to apply:** the 3D `#render-canvas` is fine filling 100% of the viewport (3D content has no
fixed aspect to preserve). Only the fixed-resolution 2D overlay needs fit/letterbox scaling. Also use
`100dvh`/`100dvw` (not `100%`/`100vh`) on html/body/canvas so mobile browser chrome and WebView system
bars don't cause stale sizing, and hook resize from `window.resize`, `visualViewport.resize`,
`orientationchange`, and a `ResizeObserver` on the canvas — any single one of these can be the only
event that fires in a given browser/WebView.
