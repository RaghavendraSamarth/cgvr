# CGVR Laboratory Programs

This repository contains practical programs for the Computer Graphics and Virtual Reality course. Each lab folder includes its implementation and output artifact.

## Case Study 2 — LumiLab: Interactive 3D Lamp Design Studio

[![Live Demo](https://img.shields.io/badge/Live%20Demo-lumilabs--one.vercel.app-blueviolet?style=for-the-badge&logo=vercel)](https://lumilabs-one.vercel.app/)

A full Unity 3D interactive virtual lighting showroom and product configurator.

- 🚀 **Live WebGL Showroom:** [https://lumilabs-one.vercel.app/](https://lumilabs-one.vercel.app/)
- 📁 **Project Folder:** [`LumiLab/`](LumiLab/)
- 🪔 **Features:** 4 procedural lamps, 3 architectural showrooms, PBR materials, custom RGB colors, lighting warmth/intensity, and Case Study 2 graphics demonstrations (2D CAD profile lathe, B-Rep hollow cylinder topology $V - E + F = 0$, live $4 \times 4$ transformation matrices, perspective projection, and wireframe mode).

## Labs 8–10

These labs use Python, PyOpenGL, and GLUT/FreeGLUT. Install dependencies from the corresponding `requirements.txt` before running the application.

| Lab | Topic | Run |
|---|---|---|
| 8 | Procedural vertex and fragment shaders | `python lab8/shader_studio.py` |
| 9 | 3D crystal collection game | `python lab9/crystal_run.py` |
| 10 | Orthographic and perspective projection | `python lab10/projection_workshop.py` |

Each of Labs 8–10 includes an `output.mp4` concept demo. Regenerate all three videos from the repository root with `python render_demos.py` after installing `video_requirements.txt`. These MP4s are rendered concept animations rather than screen recordings.
