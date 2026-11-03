"""Cohen-Sutherland line clipping with OpenGL display and PNG preview."""
import sys
from PIL import Image, ImageDraw
from OpenGL.GL import *
from OpenGL.GLU import gluOrtho2D
from OpenGL.GLUT import *

WIDTH, HEIGHT = 900, 620
WINDOW = (180, 140, 720, 480)  # xmin, ymin, xmax, ymax
LINES = [((65, 520), (370, 300)), ((340, 55), (810, 435)),
         ((260, 580), (850, 400)), ((90, 245), (810, 245)),
         ((110, 100), (700, 560)), ((430, 195), (575, 460))]
LEFT, RIGHT, BOTTOM, TOP = 1, 2, 4, 8


def outcode(x, y):
    xmin, ymin, xmax, ymax = WINDOW
    code = 0
    if x < xmin: code |= LEFT
    elif x > xmax: code |= RIGHT
    if y < ymin: code |= BOTTOM
    elif y > ymax: code |= TOP
    return code


def clip_line(p1, p2):
    x0, y0 = p1; x1, y1 = p2
    while True:
        c0, c1 = outcode(x0, y0), outcode(x1, y1)
        if not (c0 | c1):
            return (x0, y0), (x1, y1)
        if c0 & c1:
            return None
        outside = c0 or c1
        xmin, ymin, xmax, ymax = WINDOW
        if outside & TOP:
            if y1 == y0: return None
            x, y = x0 + (x1-x0)*(ymax-y0)/(y1-y0), ymax
        elif outside & BOTTOM:
            if y1 == y0: return None
            x, y = x0 + (x1-x0)*(ymin-y0)/(y1-y0), ymin
        elif outside & RIGHT:
            if x1 == x0: return None
            y, x = y0 + (y1-y0)*(xmax-x0)/(x1-x0), xmax
        else:
            if x1 == x0: return None
            y, x = y0 + (y1-y0)*(xmin-x0)/(x1-x0), xmin
        if outside == c0:
            x0, y0 = x, y
        else:
            x1, y1 = x, y


CLIPPED = [segment for line in LINES if (segment := clip_line(*line))]


def pixel(point): return point[0], HEIGHT - point[1]


def save_output(path):
    im = Image.new("RGB", (WIDTH, HEIGHT), "#0b1220")
    d = ImageDraw.Draw(im)
    xmin, ymin, xmax, ymax = WINDOW
    d.rectangle((xmin, HEIGHT-ymax, xmax, HEIGHT-ymin), outline="#54e1b1", width=3)
    for a, b in LINES:
        d.line((*pixel(a), *pixel(b)), fill="#df647c", width=2)
    for a, b in CLIPPED:
        d.line((*pixel(a), *pixel(b)), fill="#54e1b1", width=4)
    d.text((25, 20), "LAB 5  /  COHEN–SUTHERLAND LINE CLIPPING", fill="#d8e4f4")
    d.text((25, HEIGHT-30), "red: input segments     green: visible clipped segments", fill="#aab8cb")
    im.save(path)


def display():
    glClear(GL_COLOR_BUFFER_BIT)
    xmin, ymin, xmax, ymax = WINDOW
    glColor3f(0.32, 0.88, 0.69); glLineWidth(2)
    glBegin(GL_LINE_LOOP)
    for x, y in ((xmin,ymin),(xmax,ymin),(xmax,ymax),(xmin,ymax)): glVertex2i(x,y)
    glEnd()
    glEnable(GL_LINE_STIPPLE); glLineStipple(1, 0x0F0F); glColor3f(0.86,0.35,0.47)
    glBegin(GL_LINES)
    for a,b in LINES: glVertex2i(*a); glVertex2i(*b)
    glEnd(); glDisable(GL_LINE_STIPPLE)
    glColor3f(0.32,0.88,0.69); glLineWidth(4); glBegin(GL_LINES)
    for a,b in CLIPPED: glVertex2f(*a); glVertex2f(*b)
    glEnd(); glFlush()


def main():
    if len(sys.argv)>1 and sys.argv[1]=="--render":
        save_output(sys.argv[2] if len(sys.argv)>2 else "output.png"); return
    glutInit(); glutInitDisplayMode(GLUT_SINGLE|GLUT_RGB); glutInitWindowSize(WIDTH,HEIGHT)
    glutCreateWindow(b"Lab 5 - Cohen-Sutherland Clipping")
    glClearColor(0.04,0.07,0.13,1); glMatrixMode(GL_PROJECTION); glLoadIdentity(); gluOrtho2D(0,WIDTH,0,HEIGHT)
    glutDisplayFunc(display); glutMainLoop()


if __name__=="__main__": main()
