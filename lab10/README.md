# Lab 10 — Projection Workshop

## Aim

Compare parallel orthographic projection with perspective projection while keeping the objects and camera direction the same.

## What is implemented

The interactive scene renders a rotating colored cube and a teapot in two synchronized viewports. The left camera uses `glOrtho`; the right camera uses `gluPerspective`. Both use the same model rotation and viewpoint, making size change and foreshortening the main visual differences. The FOV and orthographic span can be adjusted live.

## Run

Install `requirements.txt`, then run:

```powershell
python projection_workshop.py
```

Controls: Space pauses animation, R resets parameters, `+`/`-` adjusts perspective FOV, `[`/`]` adjusts orthographic span, and Esc exits. Requires PyOpenGL, GLUT/FreeGLUT, and desktop OpenGL.

## Files

- `projection_workshop.py` — interactive comparison application.
- `output.mp4` — rendered side-by-side projection demonstration.

## Concepts

View and projection matrices, camera frusta, aspect ratios, viewport partitioning, depth buffering, perspective foreshortening, and parallel projection.
