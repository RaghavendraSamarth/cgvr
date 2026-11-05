# Lab 8 — Procedural Shader Studio

## Aim

Show how a vertex shader can deform geometry while a fragment shader computes a time-varying surface colour.

## What is implemented

The working demo draws a textured quad from a small vertex buffer. `shaders/vertex.glsl` applies a sinusoidal vertical displacement to the vertices. `shaders/fragment.glsl` combines radial waves, diagonal bands, and a soft center glow. The two stages share a time uniform, so the deformation and colour motion stay synchronized. This gives a different shader exercise from a three-colour triangle interpolation demo.

## Run

Install the packages in `requirements.txt`, then run:

```powershell
python shader_studio.py
```

Controls: Space pauses or resumes, `+` and `-` change the animation rate, `R` restarts, and Esc exits. A desktop OpenGL 3.3 compatibility context and GLUT/FreeGLUT are required.

## Files

- `shader_studio.py` — interactive OpenGL host and controls.
- `shaders/vertex.glsl` — vertex displacement and UV forwarding.
- `shaders/fragment.glsl` — radial and diagonal procedural color field.
- `output.mp4` — rendered demonstration of the shader effect.

## Concepts

Vertex attributes, clip-space transformation, uniforms, interpolation of UV coordinates, fragment shading, time-based animation, and the separation between geometry processing and pixel coloring.
