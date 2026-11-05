"""Lab 9: compact 3D crystal collection game built with PyOpenGL + GLUT."""
import math
from OpenGL.GL import *
from OpenGL.GLU import *
from OpenGL.GLUT import *

W, H = 1000, 700
player = [0.0, 0.48, 5.5]
CRYSTAL_START = [(-5.2, -4.5), (-2.4, -6.3), (2.1, -4.0), (5.0, -1.2), (0.4, 1.2), (-4.4, 2.5)]
crystals = CRYSTAL_START.copy()
keys = set()
collected = 0
started = 0
last_ms = 0
jump_v = 0.0
game_over = False


def reset():
    global player, crystals, collected, started, last_ms, jump_v, game_over
    player = [0.0, 0.48, 5.5]
    crystals = CRYSTAL_START.copy()
    collected, jump_v, game_over = 0, 0.0, False
    started = glutGet(GLUT_ELAPSED_TIME)
    last_ms = started


def text(x, y, msg, font=GLUT_BITMAP_HELVETICA_18):
    glRasterPos2f(x, y)
    for c in msg: glutBitmapCharacter(font, ord(c))


def hud():
    glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity(); gluOrtho2D(0, W, 0, H)
    glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity(); glDisable(GL_DEPTH_TEST)
    glColor3f(.87, .95, 1); text(25, H-38, "CRYSTAL RUN  /  LAB 09")
    glColor3f(.42, .93, .88)
    text(25, H-66, f"Crystals {collected} / 6     Time {(glutGet(GLUT_ELAPSED_TIME)-started)/1000:.1f}s", GLUT_BITMAP_HELVETICA_12)
    glColor3f(.82, .86, .94); text(25, 25, "WASD move    Space jump    R restart    Esc quit", GLUT_BITMAP_HELVETICA_12)
    if game_over:
        glColor3f(1.0, .82, .3); text(W*.36, H*.54, "ALL CRYSTALS FOUND!  Press R to play again")
    glEnable(GL_DEPTH_TEST); glPopMatrix(); glMatrixMode(GL_PROJECTION); glPopMatrix(); glMatrixMode(GL_MODELVIEW)


def draw_floor():
    glColor3f(.10, .16, .23); glBegin(GL_QUADS)
    glVertex3f(-8, 0, 8); glVertex3f(8, 0, 8); glVertex3f(8, 0, -8); glVertex3f(-8, 0, -8)
    glEnd(); glColor3f(.20, .29, .37); glBegin(GL_LINES)
    for i in range(-8, 9):
        glVertex3f(i, .012, -8); glVertex3f(i, .012, 8)
        glVertex3f(-8, .012, i); glVertex3f(8, .012, i)
    glEnd()


def display():
    glClearColor(.025, .045, .085, 1); glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glViewport(0, 0, W, H); glMatrixMode(GL_PROJECTION); glLoadIdentity(); gluPerspective(48, W/H, .1, 80)
    glMatrixMode(GL_MODELVIEW); glLoadIdentity()
    gluLookAt(player[0]+7, 10, player[2]+9, player[0], 0, player[2]-1.4, 0, 1, 0)
    draw_floor(); t = glutGet(GLUT_ELAPSED_TIME) / 1000
    for i, (x, z) in enumerate(crystals):
        glPushMatrix(); glTranslatef(x, .75+.15*math.sin(t*2+i), z); glRotatef(t*75+i*31, 0, 1, 0)
        glColor3f(.18, .88, .86); glutSolidOctahedron(); glPopMatrix()
    glPushMatrix(); glTranslatef(*player); glColor3f(.93, .40, .31); glutSolidSphere(.48, 24, 16); glPopMatrix()
    hud(); glutSwapBuffers()


def update():
    global last_ms, jump_v, collected, game_over
    now = glutGet(GLUT_ELAPSED_TIME); dt = min((now-last_ms)/1000, .04); last_ms = now
    if not game_over:
        dx = (('d' in keys)-('a' in keys))*4.2*dt
        dz = (('s' in keys)-('w' in keys))*4.2*dt
        length = math.hypot(dx, dz)
        if length > 0:
            player[0] = max(-7.2, min(7.2, player[0]+dx/max(1, length)))
            player[2] = max(-7.2, min(7.2, player[2]+dz/max(1, length)))
        if player[1] > .48 or jump_v > 0:
            jump_v -= 12*dt; player[1] = max(.48, player[1]+jump_v*dt)
            if player[1] == .48: jump_v = 0
        remaining = [(x,z) for x,z in crystals if math.hypot(player[0]-x, player[2]-z) >= .85]
        collected += len(crystals)-len(remaining)
        crystals[:] = remaining
        if not crystals: game_over = True
    glutPostRedisplay()


def key_down(k, _x, _y):
    global jump_v
    k = k.lower()
    if k == b"\x1b": glutLeaveMainLoop()
    elif k == b"r": reset()
    elif k == b" " and player[1] <= .481: jump_v = 6.3
    else: keys.add(k.decode("ascii", "ignore"))


def key_up(k, _x, _y): keys.discard(k.decode("ascii", "ignore").lower())


def reshape(w, h):
    global W, H
    W, H = max(w,1), max(h,1); glViewport(0,0,W,H)


def main():
    global started, last_ms
    glutInit(); glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH); glutInitWindowSize(W,H)
    glutCreateWindow(b"CGVR Lab 9 - Crystal Run")
    glEnable(GL_DEPTH_TEST); started = last_ms = glutGet(GLUT_ELAPSED_TIME)
    glutDisplayFunc(display); glutReshapeFunc(reshape); glutKeyboardFunc(key_down); glutKeyboardUpFunc(key_up); glutIdleFunc(update)
    glutMainLoop()


if __name__ == "__main__": main()
