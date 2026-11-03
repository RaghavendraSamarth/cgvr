"""Integer Bresenham line rasterization, including all octants."""
from OpenGL.GL import *
from OpenGL.GLU import gluOrtho2D
from OpenGL.GLUT import *


def bresenham_points(x0, y0, x1, y1):
    points = []
    dx, sx = abs(x1 - x0), 1 if x0 < x1 else -1
    dy, sy = -abs(y1 - y0), 1 if y0 < y1 else -1
    error = dx + dy
    while True:
        points.append((x0, y0))
        if (x0, y0) == (x1, y1):
            break
        twice = 2 * error
        if twice >= dy:
            error += dy
            x0 += sx
        if twice <= dx:
            error += dx
            y0 += sy
    return points


POINTS = bresenham_points(80, 80, 700, 480)


def draw():
    glClear(GL_COLOR_BUFFER_BIT)
    glColor3f(1.0, 0.67, 0.25)
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
    glutCreateWindow(b"Lab 1 - Bresenham Line Drawing")
    glClearColor(0.035, 0.055, 0.09, 1)
    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluOrtho2D(0, 800, 0, 600)
    glutDisplayFunc(draw)
    glutMainLoop()


if __name__ == "__main__":
    main()
