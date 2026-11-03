"""Depth-buffer visibility demo using overlapping sloped triangles."""
import sys
from PIL import Image, ImageDraw
from OpenGL.GL import *
from OpenGL.GLU import gluOrtho2D
from OpenGL.GLUT import *

WIDTH, HEIGHT = 900, 620
# With this camera, z values closer to zero are nearer than more negative values.
TRIANGLES = [
    (((145, 120, -0.15), (600, 150, -0.78), (365, 520, -0.37)),
     ((0.98, 0.38, 0.35), (1.0, 0.68, 0.30), (0.84, 0.25, 0.56))),
    (((285, 185, -0.68), (760, 200, -0.22), (510, 550, -0.82)),
     ((0.24, 0.73, 0.98), (0.32, 0.95, 0.76), (0.34, 0.48, 0.96))),
]


def edge(a, b, x, y):
    return (x-a[0])*(b[1]-a[1]) - (y-a[1])*(b[0]-a[0])


def save_output(path):
    image = Image.new("RGB", (WIDTH, HEIGHT), "#0b1220")
    pixels = image.load()
    depth = [[1.0] * WIDTH for _ in range(HEIGHT)]
    for verts, colors in TRIANGLES:
        minx = max(0, int(min(v[0] for v in verts))); maxx = min(WIDTH-1, int(max(v[0] for v in verts)))
        miny = max(0, int(min(v[1] for v in verts))); maxy = min(HEIGHT-1, int(max(v[1] for v in verts)))
        area = edge(verts[0], verts[1], verts[2][0], verts[2][1])
        for py in range(miny, maxy+1):
            for x in range(minx, maxx+1):
                y = HEIGHT - py
                w0 = edge(verts[1], verts[2], x, y) / area
                w1 = edge(verts[2], verts[0], x, y) / area
                w2 = 1.0 - w0 - w1
                if min(w0, w1, w2) < 0: continue
                z = w0*verts[0][2] + w1*verts[1][2] + w2*verts[2][2]
                d = (1.0 - z) / 2.0
                if d < depth[py][x]:
                    depth[py][x] = d
                    rgb = tuple(round(255*(w0*colors[0][i] + w1*colors[1][i] + w2*colors[2][i])) for i in range(3))
                    pixels[x, py] = rgb
    draw = ImageDraw.Draw(image)
    draw.text((24, 20), "LAB 6  /  Z-BUFFER HIDDEN-SURFACE REMOVAL", fill="#d8e4f4")
    draw.text((24, HEIGHT-30), "intersecting triangles: nearest interpolated depth wins per pixel", fill="#aab8cb")
    image.save(path)


def display():
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glEnable(GL_DEPTH_TEST); glDepthFunc(GL_LESS); glShadeModel(GL_SMOOTH)
    glBegin(GL_TRIANGLES)
    for verts, colors in TRIANGLES:
        for (x, y, z), color in zip(verts, colors):
            glColor3f(*color); glVertex3f(x, y, z)
    glEnd(); glutSwapBuffers()


def main():
    if len(sys.argv)>1 and sys.argv[1]=="--render":
        save_output(sys.argv[2] if len(sys.argv)>2 else "output.png"); return
    glutInit(); glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH); glutInitWindowSize(WIDTH,HEIGHT)
    glutCreateWindow(b"Lab 6 - Z Buffer Hidden Surface Removal")
    glClearColor(0.04,0.07,0.13,1); glClearDepth(1.0)
    glMatrixMode(GL_PROJECTION); glLoadIdentity(); glOrtho(0,WIDTH,0,HEIGHT,-1,1)
    glMatrixMode(GL_MODELVIEW); glLoadIdentity(); glutDisplayFunc(display); glutMainLoop()


if __name__=="__main__": main()
