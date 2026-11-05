"""Lab 10: compare orthographic and perspective projection of a small scene."""
from OpenGL.GL import *
from OpenGL.GLU import *
from OpenGL.GLUT import *

W,H=1100,720
angle=24.0
fov=48.0
ortho_span=3.2
running=True
last_ms=0
FACES=[
    ((0,0,1),(.16,.80,.77),[(-1,-1,1),(1,-1,1),(1,1,1),(-1,1,1)]),
    ((0,0,-1),(.70,.36,.91),[(1,-1,-1),(-1,-1,-1),(-1,1,-1),(1,1,-1)]),
    ((1,0,0),(.98,.60,.27),[(1,-1,1),(1,-1,-1),(1,1,-1),(1,1,1)]),
    ((-1,0,0),(.33,.67,.96),[(-1,-1,-1),(-1,-1,1),(-1,1,1),(-1,1,-1)]),
    ((0,1,0),(.95,.79,.30),[(-1,1,1),(1,1,1),(1,1,-1),(-1,1,-1)]),
    ((0,-1,0),(.88,.35,.43),[(-1,-1,-1),(1,-1,-1),(1,-1,1),(-1,-1,1)]),
]

def cube(size=1):
    glPushMatrix(); glScalef(size,size,size)
    for normal,color,points in FACES:
        glColor3f(*color); glBegin(GL_QUADS); glNormal3f(*normal)
        for p in points: glVertex3f(*p)
        glEnd()
    glPopMatrix()

def text(x,y,s,font=GLUT_BITMAP_HELVETICA_18):
    glRasterPos2f(x,y)
    for c in s: glutBitmapCharacter(font,ord(c))

def viewport(x,width,perspective):
    scene_y=80; scene_h=H-145
    glViewport(x,scene_y,width,scene_h); aspect=width/scene_h
    glMatrixMode(GL_PROJECTION); glLoadIdentity()
    if perspective: gluPerspective(fov,aspect,.1,100)
    else:
        half=ortho_span
        if aspect>1: glOrtho(-half*aspect,half*aspect,-half,half,.1,100)
        else: glOrtho(-half,half,-half/aspect,half/aspect,.1,100)
    glMatrixMode(GL_MODELVIEW); glLoadIdentity(); gluLookAt(5,4,7,0,0,0,0,1,0); glRotatef(angle,.35,1,.18)
    glPushMatrix(); glTranslatef(-1.1,0,0); cube(.72); glPopMatrix()
    glPushMatrix(); glTranslatef(1.15,0,-.4); glColor3f(.34,.82,.45); glutSolidTeapot(.72); glPopMatrix()

def hud():
    glViewport(0,0,W,H); glMatrixMode(GL_PROJECTION); glPushMatrix(); glLoadIdentity(); gluOrtho2D(0,W,0,H)
    glMatrixMode(GL_MODELVIEW); glPushMatrix(); glLoadIdentity(); glDisable(GL_DEPTH_TEST)
    glColor3f(.91,.95,1); text(25,H-35,"LAB 10  /  PROJECTION WORKSHOP")
    glColor3f(.42,.88,.85); text(25,H-63,"ORTHOGRAPHIC  |  parallel rays",GLUT_BITMAP_HELVETICA_12)
    text(W*.53,H-63,"PERSPECTIVE  |  converging rays",GLUT_BITMAP_HELVETICA_12)
    glColor3f(.83,.87,.95); text(25,25,f"Space pause    R reset    +/- FOV {fov:.0f} deg    [/] ortho span {ortho_span:.1f}    Esc quit",GLUT_BITMAP_HELVETICA_12)
    glColor3f(.20,.34,.43); glBegin(GL_LINES); glVertex2f(W/2,80); glVertex2f(W/2,H-90); glEnd()
    glEnable(GL_DEPTH_TEST); glPopMatrix(); glMatrixMode(GL_PROJECTION); glPopMatrix(); glMatrixMode(GL_MODELVIEW)

def display():
    glClearColor(.025,.045,.075,1); glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT)
    half=W//2; viewport(0,half,False); viewport(half,W-half,True); hud(); glutSwapBuffers()

def key(k,_x,_y):
    global running,angle,fov,ortho_span,last_ms
    if k==b"\x1b": glutLeaveMainLoop()
    elif k==b" ": running=not running
    elif k in (b"r",b"R"): angle=24; fov=48; ortho_span=3.2; running=True; last_ms=glutGet(GLUT_ELAPSED_TIME)
    elif k in (b"+",b"="): fov=min(72,fov+3)
    elif k in (b"-",b"_"): fov=max(24,fov-3)
    elif k==b"[": ortho_span=max(2.0,ortho_span-.2)
    elif k==b"]": ortho_span=min(5.0,ortho_span+.2)
    glutPostRedisplay()

def reshape(w,h):
    global W,H
    W,H=max(1,w),max(1,h)

def idle():
    global angle,last_ms
    now=glutGet(GLUT_ELAPSED_TIME); dt=min((now-last_ms)/1000,.05); last_ms=now
    if running: angle=(angle+24*dt)%360
    glutPostRedisplay()

def main():
    global last_ms
    glutInit(); glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH); glutInitWindowSize(W,H)
    glutCreateWindow(b"CGVR Lab 10 - Projection Workshop")
    glEnable(GL_DEPTH_TEST); last_ms=glutGet(GLUT_ELAPSED_TIME)
    glutDisplayFunc(display); glutReshapeFunc(reshape); glutKeyboardFunc(key); glutIdleFunc(idle); glutMainLoop()

if __name__=="__main__": main()
