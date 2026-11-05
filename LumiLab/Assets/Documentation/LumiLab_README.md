# LumiLab — Interactive 3D Lamp Design Studio

**LumiLab** is a virtual lighting showroom and interactive 3D lamp design studio built in Unity 6 with the Universal Render Pipeline (URP). It serves as both a customer-facing product configurator and an interactive Computer Graphics & Virtual Reality academic case study demonstration.

---

## 🚀 Quick Start Guide

### How to Run in Unity
1. Open the project in **Unity 6** (`6000.6.7f1` or compatible).
2. Open the main showroom scene:
   `Assets/Scenes/LumiLab_Showroom.unity`
3. Press the **Play** button in the Unity Editor toolbar.
4. **Interactive Controls**:
   - **Rotate Camera**: Click and drag with Left or Right Mouse Button.
   - **Zoom**: Scroll mouse wheel or pinch on touch screens.
   - **Switch Lamps**: Click the **☰ Collection** drawer on the left to select any of the 4 models.
   - **Customize**: Open the **Customize ⚙** drawer on the right to modify Dimensions, Materials, Curated/RGB Colors, and Lighting.
   - **Presets**: Choose from 5 curated mood presets (*Cozy Evening*, *Modern Minimal*, *Luxury Gold*, *Industrial Black*, *Colorful Ambient*).
   - **Environments**: Use the top-bar dropdown to switch between *Luxury Living Room*, *Minimal Design Studio*, and *Cozy Bedroom*.
   - **Case Study 2 Demonstration**: Click the **Case Study 2** button in the top navigation bar to inspect real-time profile revolution, B-Rep hollow manifold geometry, 4×4 homogeneous transformation matrices, camera projection matrices, and wireframe depth-buffer inspection.
   - **Save & Load**: Use **💾 Save Design** and **📂 Load Design** to persist and restore custom lamp configurations.

### Editor Menu Tool
You can also regenerate or rebuild the entire showroom scene anytime from the Unity Editor menu:
- **Menu Bar**: `LumiLab → Build Complete Showroom`

---

## 🛋️ Features Overview

### 1. 4 Distinct Procedural Lamp Models
1. **Aura Modern**: Sleek brushed metal circular base, slender upright stand, tapered/fluted fabric-textured lampshade with warm interior downlight.
2. **Nordic Glow**: Scandinavian warm walnut base, organic rounded ceramic gourd body (profile-revolved), and textured linen drum shade.
3. **Industrial Edge**: Heavy gunmetal stepped base, articulated cantilever double-pivot arms with elbow brackets, and an angled spotlight head casting focused directional illumination.
4. **Halo Contemporary**: Futuristic polished platinum base, slender arch stem, and a 360-degree luminous torus halo ring diffuser.

### 2. Multi-Component Customization
- **Dimensions**: Proportional scaling of Lamp Height, Base Width, Shade Width, Shade Height, and Shade Profile Styles.
- **Physical Materials**: Brushed Metal, Polished Metal, Matte Ceramic, Walnut Wood, Frosted Glass, and Woven Fabric.
- **Colors**: Curated interior-designer swatches + full RGB precision sliders.
- **Lighting Controls**: Independent On/Off toggle, brightness intensity slider, and calibrated color temperatures:
  - Warm White ($2700\text{ K}$)
  - Neutral White ($4000\text{ K}$)
  - Cool White ($6500\text{ K}$)
  - Custom RGB ambient illumination

### 3. Three 3D Showroom Environments
- **Luxury Living Room**: Architectural dark-wood slatted feature wall, hardwood parquet floor, marble-and-brass side table, and warm ambient fill.
- **Minimal Design Studio**: Seamless curved cyclorama photography backdrop, cylindrical display pedestal, and 3-point studio illumination.
- **Cozy Bedroom**: Soft greige wall, modern upholstered bed with headboard and duvet, and realistic bedside nightstand placement.

### 4. Interactive Camera System
- Smooth damped mouse/touch orbit and zoom.
- Collision and elevation safety clamps ($5^\circ$ to $85^\circ$) preventing camera clipping below floors.
- One-click presets: 3/4 Perspective (default), Front Elevation, Side Profile, Top Plan, and Reset.
- Automated turntable auto-rotate toggle.

### 5. Case Study 2 Graphics Demonstrations
- **2D CAD Profile Revolution**: Real-time parametric lathe sweep with adjustable radial tessellation ($N = 6 \dots 48$).
- **Boundary Representation (B-Rep)**: Explicit 4-surface manifold hollow cylinder with outward/inward face normals.
- **Geometric Transformations**: Live numerical composite $4 \times 4$ homogeneous transformation matrix $M = T \cdot R \cdot S$.
- **Viewing & Projection**: Perspective vs Orthographic projection, FOV adjustment, near/far clipping plane slicing, and depth-buffer (Z-buffer) visualization.

