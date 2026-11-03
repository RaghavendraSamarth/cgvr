"""Compare two transformation orders using homogeneous matrices."""
import sys
import numpy as np
from PIL import Image, ImageDraw
from OpenGL.GL import *
from OpenGL.GLU import gluOrtho2D
from OpenGL.GLUT import *

WIDTH, HEIGHT = 900, 620
SHAPE = np.array([[-65, -45, 1], [65, -45, 1], [65, 45, 1], [-65, 45, 1]], float).T


def translate(x, y):
    return np.array([[1, 0, x], [0, 1, y], [0, 0, 1]], float)


def rotate(degrees):
    a = np.deg2rad(degrees)
    return np.array([[np.cos(a), -np.sin(a), 0], [np.sin(a), np.cos(a), 0], [0, 0, 1]])


def scale(x, y):
    return np.diag([x, y, 1.0])


LEFT = translate(-215, 0) @ rotate(28) @ scale(1.3, 0.75)
RIGHT = rotate(28) @ scale(1.3, 0.75) @ translate(215, 0)


def to_screen(x, y):
    return int(WIDTH / 2 + x), int(HEIGHT / 2 - y)


def save_output(path):
    im = Image.new("RGB", (WIDTH, HEIGHT), "#0b1220")
    d = ImageDraw.Draw(im)
    d.line((WIDTH//2, 40, WIDTH//2, HEIGHT-40), fill="#394a61", width=2)
    for cx in (WIDTH//4, 3*WIDTH//4):
        d.line((cx, 48, cx, HEIGHT-45), fill="#1d2b40")
        d.line((35 if cx < WIDTH//2 else WIDTH//2+35, HEIGHT//2,
                WIDTH//2-35 if cx < WIDTH//2 else WIDTH-35, HEIGHT//2), fill="#1d2b40")
    for mat, color in ((LEFT, "#54e1b1"), (RIGHT, "#f6a75b")):
        pts = [to_screen(*(mat @ p)[:2]) for p in SHAPE.T]
        d.polygon(pts, fill="#17273a", outline=color, width=4)
    d.text((26, 20), "LAB 4  /  COMPOSITE TRANSFORMATIONS", fill="#d8e4f4")
    d.text((WIDTH//4-128, HEIGHT-32), "T · R · S   (scale, rotate, translate)", fill="#54e1b1")
    d.text((3*WIDTH//4-138, HEIGHT-32), "R · S · T   (translate, scale, rotate)", fill="#f6a75b")
    im.save(path)


def draw_poly(matrix, color):
    glColor3f(*color)
    glBegin(GL_POLYGON)
    for p in SHAPE.T:
        x, y, _ = matrix @ p
        glVertex2f(x, y)
    glEnd()


def display():
    glClear(GL_COLOR_BUFFER_BIT)
    draw_poly(LEFT, (0.33, 0.88, 0.70))
    draw_poly(RIGHT, (0.96, 0.65, 0.35))
    glFlush()


def main():
    if len(sys.argv) > 1 and sys.argv[1] == "--render":
        save_output(sys.argv[2] if len(sys.argv) > 2 else "output.png"); return
    glutInit(); glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
    glutInitWindowSize(WIDTH, HEIGHT); glutCreateWindow(b"Lab 4 - Composite Transformations")
    glClearColor(0.04, 0.07, 0.13, 1)
    glMatrixMode(GL_PROJECTION); glLoadIdentity(); gluOrtho2D(-WIDTH/2, WIDTH/2, -HEIGHT/2, HEIGHT/2)
    glutDisplayFunc(display); glutMainLoop()


if __name__ == "__main__":
    main()
