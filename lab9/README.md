# Lab 9 — Crystal Run

## Aim

Build a small interactive 3D collection game and connect movement, animation, collision, score, and elapsed time.

## Gameplay

Guide the red player sphere around a grid arena and touch six floating crystals. WASD moves, Space jumps, R restarts, and Esc exits. The game tracks the score and run time; collecting the final crystal displays a completion message. The scene is built from procedural OpenGL primitives, so no external models or Unity assets are required.

## Run

Install `requirements.txt`, then run:

```powershell
python crystal_run.py
```

Requires Python 3.10+, PyOpenGL, GLUT/FreeGLUT, and a desktop OpenGL context.

## Files

- `crystal_run.py` — complete playable game.
- `output.mp4` — rendered demo walkthrough.

## Graphics concepts

Perspective camera, world transforms, object animation, keyboard input, simple sphere-to-point collision checks, jump integration, depth buffering, and real-time HUD updates.
