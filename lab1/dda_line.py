"""DDA line rasterization shown with OpenGL/GLUT."""
from OpenGL.GL import *
from OpenGL.GLU import gluOrtho2D
from OpenGL.GLUT import *


def dda_points(x0, y0, x1, y1):
    dx, dy = x1 - x0, y1 - y0
    steps = max(abs(dx), abs(dy))
    if steps == 0:
        return [(round(x0), round(y0))]
    return [(round(x0 + i * dx / steps), round(y0 + i * dy / steps))
            for i in range(steps + 1)]


POINTS = dda_points(80, 80, 700, 480)


def draw():
    glClear(GL_COLOR_BUFFER_BIT)
    glColor3f(0.18, 0.88, 0.65)
    glPointSize(5)
    glBegin(GL_POINTS)
    for x, y in POINTS:
        glVertex2i(x, y)
    glEnd()
    glFlush()


def main():
    glutInit()
    glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
    glutInitWindowSize(800, 600)
    glutCreateWindow(b"Lab 1 - DDA Line Drawing")
    glClearColor(0.035, 0.055, 0.09, 1)
    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluOrtho2D(0, 800, 0, 600)
    glutDisplayFunc(draw)
    glutMainLoop()


if __name__ == "__main__":
    main()
