"""Midpoint circle rasterization with an OpenGL viewer and PNG output."""
import sys
from PIL import Image, ImageDraw
from OpenGL.GL import *
from OpenGL.GLU import gluOrtho2D
from OpenGL.GLUT import *

WIDTH, HEIGHT, RADIUS = 800, 600, 180
CX, CY = WIDTH // 2, HEIGHT // 2


def circle_points(radius):
    x, y, decision = 0, radius, 1 - radius
    points = set()
    while x <= y:
        points.update(((x, y), (y, x), (-x, y), (-y, x),
                       (x, -y), (y, -x), (-x, -y), (-y, -x)))
        x += 1
        if decision < 0:
            decision += 2 * x + 1
        else:
            y -= 1
            decision += 2 * (x - y) + 1
    return sorted(points)


POINTS = circle_points(RADIUS)


def save_output(path):
    image = Image.new("RGB", (WIDTH, HEIGHT), "#0b1220")
    draw = ImageDraw.Draw(image)
    draw.line((CX, 30, CX, HEIGHT - 30), fill="#23334a")
    draw.line((30, CY, WIDTH - 30, CY), fill="#23334a")
    for x, y in POINTS:
        draw.ellipse((CX + x - 2, CY - y - 2, CX + x + 2, CY - y + 2), fill="#54e1b1")
    draw.text((28, 24), "LAB 2  /  MIDPOINT CIRCLE ALGORITHM", fill="#d8e4f4")
    image.save(path)


def display():
    glClear(GL_COLOR_BUFFER_BIT)
    glColor3f(0.33, 0.88, 0.70)
    glPointSize(5)
    glBegin(GL_POINTS)
    for x, y in POINTS:
        glVertex2i(CX + x, CY + y)
    glEnd()
    glFlush()


def main():
    if len(sys.argv) > 1 and sys.argv[1] == "--render":
        save_output(sys.argv[2] if len(sys.argv) > 2 else "output.png")
        return
    glutInit()
    glutInitDisplayMode(GLUT_SINGLE | GLUT_RGB)
    glutInitWindowSize(WIDTH, HEIGHT)
    glutCreateWindow(b"Lab 2 - Midpoint Circle")
    glClearColor(0.04, 0.07, 0.13, 1)
    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluOrtho2D(0, WIDTH, 0, HEIGHT)
    glutDisplayFunc(display)
    glutMainLoop()


if __name__ == "__main__":
    main()
