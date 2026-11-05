# LumiLab — Comprehensive Verification & Test Checklist

This checklist verifies all functional, visual, and mathematical requirements of **LumiLab — Interactive 3D Lamp Design Studio**.

| ID | Category | Test Item | Verification Procedure | Expected Result | Status |
|---|---|---|---|---|---|
| **01** | **Compilation** | Script compilation in Unity 6 | Launch Unity Editor or run compiler check | Zero CS compiler errors, warnings addressed | **PASS** |
| **02** | **Scene Setup** | Default scene registration | Check `EditorBuildSettings.asset` | `Assets/Scenes/LumiLab_Showroom.unity` is at index 0 | **PASS** |
| **03** | **Automation** | Editor Menu Item | Check Unity menu bar | `LumiLab → Build Complete Showroom` is accessible | **PASS** |
| **04** | **Lamp 1** | Aura Modern Model | Select Lamp 1 in collection | Tapered shade, slim stand, circular base, warm interior glow | **PASS** |
| **05** | **Lamp 2** | Nordic Glow Model | Select Lamp 2 in collection | Organic ceramic body (lathed), wooden base, drum shade | **PASS** |
| **06** | **Lamp 3** | Industrial Edge Model | Select Lamp 3 in collection | Stepped metallic base, multi-pivot arm, directional spotlight | **PASS** |
| **07** | **Lamp 4** | Halo Contemporary Model | Select Lamp 4 in collection | Torus ring light diffuser, arch stem, 360° radial illumination | **PASS** |
| **08** | **Selection** | Single active lamp visibility | Switch between all 4 models | Only the selected lamp appears; others deactivated | **PASS** |
| **09** | **Dimensions** | Height, Base, Shade Width/Height | Drag sliders in Dimensions tab | Geometry updates smoothly without breaking component joints | **PASS** |
| **10** | **Materials** | 6 PBR physical materials | Switch Base, Stand, Shade dropdowns | Brushed Metal, Polished, Ceramic, Wood, Glass, Fabric update | **PASS** |
| **11** | **Colors** | Swatches & RGB sliders | Click swatch or adjust R, G, B sliders | Base, Stand, or Shade color updates in real time | **PASS** |
| **12** | **Lighting** | Lamp Power On/Off toggle | Toggle power switch in UI or Bottom Bar | Lamp light and emissive bulb turn off/on; room darkens | **PASS** |
| **13** | **Brightness** | Intensity scaling | Drag Brightness slider (0.1 to 2.5) | Visible illuminance on nearby surfaces increases/decreases | **PASS** |
| **14** | **Color Temp** | Warm, Neutral, Cool white | Click 2700K, 4000K, 6500K buttons | Light and bulb emission tint shift accurately | **PASS** |
| **15** | **Presets** | 5 Curated Mood Presets | Click Cozy Evening, Modern Minimal, etc. | Preset updates materials, colors, and lighting simultaneously | **PASS** |
| **16** | **Showrooms** | Luxury Living Room | Select Living Room in dropdown | Hardwood floor, slatted accent wall, marble side table | **PASS** |
| **17** | **Showrooms** | Minimal Design Studio | Select Design Studio in dropdown | Seamless cyclorama backdrop, display plinth, 3-point lights | **PASS** |
| **18** | **Showrooms** | Cozy Bedroom | Select Bedroom in dropdown | Nightstand table, upholstered bed, evening ambient tone | **PASS** |
| **19** | **Camera** | Orbit & Zoom Controls | Left/Right drag to rotate, scroll to zoom | Camera moves smoothly with damping; no jump cuts | **PASS** |
| **20** | **Camera** | Collision & Floor Limits | Drag camera downward | Pitch is clamped ($5^\circ$ to $85^\circ$); cannot clip below floor | **PASS** |
| **21** | **Camera** | Preset Angles | Click 3/4, Front, Side, Top, Reset | Camera animates smoothly to standard architectural angles | **PASS** |
| **22** | **Camera** | UI Blocking Check | Drag mouse over UI panels | Camera does not orbit while interacting with sliders/buttons | **PASS** |
| **23** | **Camera** | Turntable Auto-Rotate | Toggle Auto-Rotate in bottom bar | Camera slowly revolves around lamp continuously | **PASS** |
| **24** | **Persistence**| Local Save & Load | Click Save Design, reload lamp, click Load | User configuration persists across selections and restarts | **PASS** |
| **25** | **Case Study 2**| 2D CAD Lathe Demonstration | Open Tech Demo, adjust Tessellation | Mesh recalculates vertex/triangle count and compute time | **PASS** |
| **26** | **Case Study 2**| B-Rep Hollow Manifold Solid | Switch to B-Rep mode in Tech Demo | Displays explicit inner/outer cylinder surfaces with normals | **PASS** |
| **27** | **Case Study 2**| 4×4 Transformation Matrix | Adjust Scale, Rotation, Translation | Live $4 \times 4$ homogeneous matrix $M = T \cdot R \cdot S$ updates | **PASS** |
| **28** | **Case Study 2**| Projection & Z-Buffer View | Adjust FOV, toggle Wireframe, Near Clip | Displays Projection matrix and Z-buffer occlusion equation | **PASS** |
| **29** | **Performance**| Frame Rate & Memory HUD | Observe HUD in Tech Demo panel | Constant $>60\text{ FPS}$, stable memory allocation | **PASS** |

