"""2D homogeneous transformations with an OpenGL viewer and PNG preview."""
import sys
import numpy as np
from PIL import Image, ImageDraw
from OpenGL.GL import *
from OpenGL.GLU import gluOrtho2D
from OpenGL.GLUT import *

WIDTH, HEIGHT = 900, 640
SOURCE = np.array([[-100, -70, 1], [100, -70, 1], [100, 70, 1], [-100, 70, 1]], dtype=float).T


def matrix(operation):
    if operation == "translate":
        return np.array([[1, 0, 210], [0, 1, 100], [0, 0, 1]], float)
    if operation == "rotate":
        a = np.deg2rad(32)
        return np.array([[np.cos(a), -np.sin(a), 0], [np.sin(a), np.cos(a), 0], [0, 0, 1]])
    if operation == "scale":
        return np.diag([1.35, 0.72, 1.0])
    if operation == "reflect":
        return np.diag([-1.0, 1.0, 1.0])
    if operation == "shear":
        return np.array([[1, 0.55, 0], [0.15, 1, 0], [0, 0, 1]], float)
    # Translate a rotated and non-uniformly scaled object.
    return matrix("translate") @ matrix("rotate") @ matrix("scale")


OPERATION = "composite"
TRANSFORMED = matrix(OPERATION) @ SOURCE


def pixel(point):
    return int(WIDTH / 2 + point[0]), int(HEIGHT / 2 - point[1])


def save_output(path):
    image = Image.new("RGB", (WIDTH, HEIGHT), "#0b1220")
    d = ImageDraw.Draw(image)
    d.line((WIDTH // 2, 40, WIDTH // 2, HEIGHT - 35), fill="#26364b")
    d.line((40, HEIGHT // 2, WIDTH - 40, HEIGHT // 2), fill="#26364b")
    original = [pixel(SOURCE[:2, i]) for i in range(SOURCE.shape[1])]
    moved = [pixel(TRANSFORMED[:2, i]) for i in range(TRANSFORMED.shape[1])]
    d.polygon(original, outline="#8291a8", width=3)
    d.polygon(moved, fill="#30213f", outline="#e879f9", width=4)
    d.text((24, 20), "LAB 3  /  HOMOGENEOUS 2D TRANSFORMATIONS", fill="#d8e4f4")
    d.text((24, HEIGHT - 28), "gray: original     magenta: translated · rotated · scaled", fill="#aab8cb")
    image.save(path)


def display():
    glClear(GL_COLOR_BUFFER_BIT)
    glLineWidth(2)
    glColor3f(0.5, 0.58, 0.68)
    glBegin(GL_LINE_LOOP)
    for x, y, _ in SOURCE.T:
        glVertex2f(x, y)
    glEnd()
    glColor3f(0.9, 0.45, 0.97)
    glBegin(GL_POLYGON)
    for x, y, _ in TRANSFORMED.T:
        glVertex2f(x, y)
    glEnd()
    glColor3f(0.95, 0.95, 0.98)
    glBegin(GL_LINES)
    glVertex2f(-WIDTH / 2, 0); glVertex2f(WIDTH / 2, 0)
    glVertex2f(0, -HEIGHT / 2); glVertex2f(0, HEIGHT / 2)
    glEnd()
    glFlush()


def main():
    global OPERATION, TRANSFORMED
    if len(sys.argv) > 1 and sys.argv[1] == "--render":
        save_output(sys.argv[2] if len(sys.argv) > 2 else "output.png")
        return
    glutInit(); glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
    glutInitWindowSize(WIDTH, HEIGHT); glutCreateWindow(b"Lab 3 - 2D Transformations")
    glClearColor(0.04, 0.07, 0.13, 1)
    glMatrixMode(GL_PROJECTION); glLoadIdentity(); gluOrtho2D(-WIDTH/2, WIDTH/2, -HEIGHT/2, HEIGHT/2)
    glutDisplayFunc(display); glutMainLoop()


if __name__ == "__main__":
    main()
