"""Lab 8: animate a procedural surface with a GLSL vertex/fragment pair."""
from pathlib import Path
import ctypes

from OpenGL.GL import *
from OpenGL.GLU import *
from OpenGL.GLUT import *
from OpenGL.GL.shaders import compileProgram, compileShader

WIDTH, HEIGHT = 1000, 700
program = vao = vbo = None
paused = False
speed = 1.0
start_ms = 0
aspect = WIDTH / HEIGHT


def load_shader(name):
    return (Path(__file__).parent / "shaders" / name).read_text(encoding="utf-8")


def init():
    global program, vao, vbo
    program = compileProgram(
        compileShader(load_shader("vertex.glsl"), GL_VERTEX_SHADER),
        compileShader(load_shader("fragment.glsl"), GL_FRAGMENT_SHADER),
    )
    vertices = (GLfloat * 24)(
        -0.88, -0.70, 0, 0, 0.88, -0.70, 1, 0, 0.88, 0.70, 1, 1,
        -0.88, -0.70, 0, 0, 0.88, 0.70, 1, 1, -0.88, 0.70, 0, 1,
    )
    vao = glGenVertexArrays(1)
    vbo = glGenBuffers(1)
    glBindVertexArray(vao)
    glBindBuffer(GL_ARRAY_BUFFER, vbo)
    glBufferData(GL_ARRAY_BUFFER, ctypes.sizeof(vertices), vertices, GL_STATIC_DRAW)
    stride = 4 * ctypes.sizeof(GLfloat)
    glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(0))
    glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, stride, ctypes.c_void_p(2 * ctypes.sizeof(GLfloat)))
    glEnableVertexAttribArray(0)
    glEnableVertexAttribArray(1)
    glBindVertexArray(0)
    glClearColor(0.018, 0.025, 0.055, 1)


def text(x, y, value, font=GLUT_BITMAP_HELVETICA_18):
    glRasterPos2f(x, y)
    for char in value:
        glutBitmapCharacter(font, ord(char))


def overlay():
    glUseProgram(0)
    glMatrixMode(GL_PROJECTION)
    glPushMatrix(); glLoadIdentity(); gluOrtho2D(0, WIDTH, 0, HEIGHT)
    glMatrixMode(GL_MODELVIEW)
    glPushMatrix(); glLoadIdentity()
    glColor3f(0.88, 0.94, 1.0)
    text(28, HEIGHT - 42, "LAB 08  /  PROCEDURAL SHADER STUDIO")
    glColor3f(0.48, 0.88, 0.92)
    text(28, HEIGHT - 69, "Vertex stage bends the surface  |  Fragment stage paints animated rings")
    glColor3f(0.84, 0.87, 0.98)
    state = "PAUSED" if paused else "LIVE"
    text(28, 30, f"{state}    Speed {speed:.1f}x    Space pause    R reset    +/- speed    Esc quit", GLUT_BITMAP_HELVETICA_12)
    glPopMatrix(); glMatrixMode(GL_PROJECTION); glPopMatrix(); glMatrixMode(GL_MODELVIEW)


def display():
    now = glutGet(GLUT_ELAPSED_TIME)
    seconds = display.frozen_time if paused else (now - start_ms) / 1000.0 * speed
    if not paused:
        display.frozen_time = seconds
    glClear(GL_COLOR_BUFFER_BIT)
    glUseProgram(program)
    glUniform1f(glGetUniformLocation(program, "uTime"), seconds)
    glUniform1f(glGetUniformLocation(program, "uAspect"), aspect)
    glBindVertexArray(vao); glDrawArrays(GL_TRIANGLES, 0, 6); glBindVertexArray(0)
    overlay(); glutSwapBuffers()
display.frozen_time = 0.0


def reshape(w, h):
    global WIDTH, HEIGHT, aspect
    WIDTH, HEIGHT = max(1, w), max(1, h)
    aspect = WIDTH / HEIGHT
    glViewport(0, 0, WIDTH, HEIGHT)


def keyboard(key, _x, _y):
    global paused, start_ms, speed
    if key == b"\x1b": glutLeaveMainLoop()
    elif key == b" ":
        paused = not paused
        if not paused:
            start_ms = glutGet(GLUT_ELAPSED_TIME) - int(display.frozen_time / max(speed, .01) * 1000)
    elif key in (b"+", b"="): speed = min(3.0, speed + .2)
    elif key in (b"-", b"_"): speed = max(.2, speed - .2)
    elif key in (b"r", b"R"):
        start_ms = glutGet(GLUT_ELAPSED_TIME); display.frozen_time = 0.0; paused = False
    glutPostRedisplay()


def main():
    global start_ms
    glutInit(); glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGBA); glutInitWindowSize(WIDTH, HEIGHT)
    glutInitContextVersion(3, 3); glutInitContextProfile(GLUT_COMPATIBILITY_PROFILE)
    glutCreateWindow(b"CGVR Lab 8 - Procedural Shader Studio")
    init(); start_ms = glutGet(GLUT_ELAPSED_TIME)
    glutDisplayFunc(display); glutReshapeFunc(reshape); glutKeyboardFunc(keyboard); glutIdleFunc(glutPostRedisplay)
    glutMainLoop()


if __name__ == "__main__": main()
