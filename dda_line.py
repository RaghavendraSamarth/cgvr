from OpenGL.GL import *
from OpenGL.GLUT import *
from OpenGL.GLU import *

points = []


def dda_line(x1, y1, x2, y2):
    dx = abs(x2 - x1)
    dy = abs(y2 - y1)

    steps = max(dx, dy)

    xinc = (x2 - x1) / steps
    yinc = (y2 - y1) / steps

    x, y = x1, y1

    points.clear()

    for i in range(steps + 1):
        points.append((round(x), round(y)))
        x += xinc
        y += yinc

    return points


def display():
    glClear(GL_COLOR_BUFFER_BIT)
    glColor3f(0, 1, 0)
    glPointSize(8)

    glBegin(GL_POINTS)
    for x, y in points:
        glVertex2f(x, y)
    glEnd()

    glFlush()


def init():
    glClearColor(0, 0, 0, 1)

    xmin = min(x for x, y in points) - 2
    xmax = max(x for x, y in points) + 2
    ymin = min(y for x, y in points) - 2
    ymax = max(y for x, y in points) + 2

    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluOrtho2D(0, xmax+100, 0, ymax+100)


points = dda_line(100,500,700,100)
# print(points)

glutInit()
glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
glutInitWindowSize(800, 600)
glutCreateWindow(b"DDA Line Algorithm")

init()
glutDisplayFunc(display)
glutMainLoop()