"""Phong illumination components and combined shading using OpenGL lighting."""
import sys
import numpy as np
from PIL import Image, ImageDraw
from OpenGL.GL import *
from OpenGL.GLU import gluPerspective, gluLookAt
from OpenGL.GLUT import *

WIDTH, HEIGHT = 1000, 620
LIGHT = np.array([-0.45, 0.65, 0.62], float)
LIGHT /= np.linalg.norm(LIGHT)
VIEW = np.array([0.0, 0.0, 1.0])
MATERIALS = [
    ("Ambient", (0.05, 0.35, 0.9), (0, 0, 0), (0, 0, 0), 8),
    ("Diffuse", (0, 0, 0), (0.15, 0.58, 0.95), (0, 0, 0), 8),
    ("Specular", (0.025, 0.025, 0.035), (0, 0, 0), (0.95, 0.98, 1.0), 36),
    ("Phong: combined", (0.04, 0.16, 0.32), (0.08, 0.42, 0.84), (0.9, 0.95, 1.0), 36),
]


def render_preview(path):
    image = Image.new("RGB", (WIDTH, HEIGHT), "#0b1220")
    pixels = image.load()
    centers = [125, 375, 625, 875]
    radius = 102
    for cx, (label, ambient, diffuse, specular, shininess) in zip(centers, MATERIALS):
        for py in range(HEIGHT//2-radius, HEIGHT//2+radius):
            for px in range(cx-radius, cx+radius):
                nx = (px-cx)/radius
                ny = (HEIGHT//2-py)/radius
                r2 = nx*nx + ny*ny
                if r2 > 1: continue
                normal = np.array([nx, ny, np.sqrt(1-r2)])
                ndotl = max(float(np.dot(normal, LIGHT)), 0.0)
                reflected = 2*ndotl*normal-LIGHT
                spec = max(float(np.dot(reflected, VIEW)), 0.0) ** shininess if ndotl > 0 else 0.0
                rgb = [min(1.0, ambient[i]*0.28 + diffuse[i]*ndotl*0.92 + specular[i]*spec*0.85) for i in range(3)]
                # A restrained rim keeps the sphere readable outside the light lobe.
                pixels[px, py] = tuple(round(255*c) for c in rgb)
    draw = ImageDraw.Draw(image)
    draw.text((25, 20), "LAB 7  /  PHONG ILLUMINATION MODEL", fill="#d8e4f4")
    for cx, item in zip(centers, MATERIALS):
        box = draw.textbbox((0, 0), item[0])
        draw.text((cx-(box[2]-box[0])//2, HEIGHT-58), item[0], fill="#aab8cb")
    image.save(path)


def set_material(ambient, diffuse, specular, shininess):
    glMaterialfv(GL_FRONT, GL_AMBIENT, (*ambient, 1))
    glMaterialfv(GL_FRONT, GL_DIFFUSE, (*diffuse, 1))
    glMaterialfv(GL_FRONT, GL_SPECULAR, (*specular, 1))
    glMaterialf(GL_FRONT, GL_SHININESS, shininess)


def display():
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glMatrixMode(GL_MODELVIEW); glLoadIdentity(); gluLookAt(0,0,12, 0,0,0, 0,1,0)
    glLightfv(GL_LIGHT0, GL_POSITION, (-4, 6, 8, 1))
    glEnable(GL_LIGHTING); glEnable(GL_LIGHT0); glEnable(GL_DEPTH_TEST)
    glLightfv(GL_LIGHT0, GL_AMBIENT, (0.4,0.4,0.4,1))
    glLightfv(GL_LIGHT0, GL_DIFFUSE, (0.95,0.95,0.95,1))
    glLightfv(GL_LIGHT0, GL_SPECULAR, (1,1,1,1))
    for x, (label, ambient, diffuse, specular, shininess) in zip((-4.5,-1.5,1.5,4.5), MATERIALS):
        glPushMatrix(); glTranslatef(x, 0, 0); set_material(ambient,diffuse,specular,shininess)
        glutSolidSphere(0.92, 72, 48); glPopMatrix()
    glutSwapBuffers()


def main():
    if len(sys.argv)>1 and sys.argv[1]=="--render":
        render_preview(sys.argv[2] if len(sys.argv)>2 else "output.png"); return
    glutInit(); glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH); glutInitWindowSize(WIDTH,HEIGHT)
    glutCreateWindow(b"Lab 7 - Phong Illumination")
    glClearColor(0.04,0.07,0.13,1); glEnable(GL_DEPTH_TEST); glShadeModel(GL_SMOOTH)
    glMatrixMode(GL_PROJECTION); glLoadIdentity(); gluPerspective(42, WIDTH/HEIGHT, 0.1, 100)
    glutDisplayFunc(display); glutMainLoop()


if __name__=="__main__": main()
