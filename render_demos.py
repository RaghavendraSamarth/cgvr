"""Render short original MP4 explainers for Labs 8, 9, and 10.

These are deterministic visual demos of the interactive programs, rendered
without screen recording so they can be regenerated on any supported desktop.
"""
from pathlib import Path
import math

import cv2
import numpy as np
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).parent
WIDTH, HEIGHT, FPS, FRAMES = 960, 540, 24, 144
FONT_PATH = Path("C:/Windows/Fonts/segoeui.ttf")
FONT_BOLD_PATH = Path("C:/Windows/Fonts/segoeuib.ttf")


def font(size=20, bold=False):
    path = FONT_BOLD_PATH if bold else FONT_PATH
    try:
        return ImageFont.truetype(str(path), size)
    except OSError:
        return ImageFont.load_default()


def base(title, subtitle):
    im = Image.new("RGB", (WIDTH, HEIGHT), (8, 14, 29))
    d = ImageDraw.Draw(im)
    d.text((40, 28), title, fill=(235, 243, 255), font=font(27, True))
    d.text((42, 68), subtitle, fill=(132, 190, 205), font=font(16))
    return im, d


def write_mp4(path, frames):
    writer = cv2.VideoWriter(str(path), cv2.VideoWriter_fourcc(*"mp4v"), FPS, (WIDTH, HEIGHT))
    if not writer.isOpened():
        raise RuntimeError(f"Could not create MP4: {path}")
    for frame in frames:
        writer.write(cv2.cvtColor(np.asarray(frame), cv2.COLOR_RGB2BGR))
    writer.release()


def lab8_frame(i):
    t = i / FPS
    im, d = base("LAB 08  /  PROCEDURAL SHADER STUDIO", "Vertex displacement + fragment color field")
    x0, y0, x1, y1 = 48, 120, 912, 455
    gw, gh = x1-x0, y1-y0
    u, v = np.meshgrid(np.linspace(0, 1, gw), np.linspace(0, 1, gh))
    rings = .5 + .5*np.sin(24*np.sqrt((u-.5)**2+(v-.5)**2)-t*2.5)
    bands = .5 + .5*np.sin(u*17+v*11+t)
    deep = np.array([9, 14, 46.], dtype=np.float32)
    violet = np.array([178, 61, 245.], dtype=np.float32)
    cyan = np.array([26, 225, 224.], dtype=np.float32)
    blend = np.clip((rings-.12)/.83, 0, 1)[...,None]
    colors = deep*(1-blend)+violet*blend
    cblend = (np.clip((bands-.79)/.19,0,1)*.78)[...,None]
    colors = colors*(1-cblend)+cyan*cblend
    glow = np.exp(-10*((u-.5)**2+(v-.5)**2))[...,None]
    colors = np.clip(colors+glow*np.array([46,51,71]),0,255).astype(np.uint8)
    im.paste(Image.fromarray(colors), (x0,y0))
    d = ImageDraw.Draw(im)
    wave=[]
    for px in range(x0,x1,4):
        xx=(px-x0)/gw
        py=y0+int(13+9*math.sin(xx*math.tau*2+t*1.7))
        wave.append((px,py))
    d.line(wave, fill=(250,232,255), width=3)
    d.rounded_rectangle((48,474,912,514), radius=12, fill=(18,28,47), outline=(44,69,94))
    d.text((68,484), "uTime  →  moving bands     |     vertex wave  →  changing silhouette", fill=(212,226,241), font=font(16))
    return im


def iso(x,z):
    return (480 + (x-z)*35, 335 + (x+z)*18)


def gem(draw, x, z, t, collected=False):
    px,py=iso(x,z)
    bob=math.sin(t*2+x)*5
    top=(px,py-30-bob); left=(px-15,py-7-bob); right=(px+15,py-7-bob); tip=(px,py+12-bob)
    if collected:
        draw.ellipse((px-9,py-2,px+9,py+16),outline=(93,124,148),width=2)
        return
    draw.polygon((top,left,(px,py-8-bob)),fill=(110,255,239))
    draw.polygon((top,(px,py-8-bob),right),fill=(39,198,203))
    draw.polygon((left,tip,right,(px,py-8-bob)),fill=(31,130,165))
    draw.line((top,left,tip,right,top),fill=(192,255,247),width=2)


def lab9_frame(i):
    t=i/FPS
    im,d=base("LAB 09  /  CRYSTAL RUN", "Six pickups  •  movement  •  collision  •  scoring")
    # Isometric arena polygon and tile grid.
    corners=[iso(-7,-7),iso(7,-7),iso(7,7),iso(-7,7)]
    d.polygon(corners,fill=(23,43,59),outline=(81,117,133))
    for n in range(-7,8):
        d.line((iso(n,-7),iso(n,7)),fill=(43,69,83),width=1)
        d.line((iso(-7,n),iso(7,n)),fill=(43,69,83),width=1)
    spots=[(-5.2,-4.5),(-2.4,-6.3),(2.1,-4.0),(5.0,-1.2),(.4,1.2),(-4.4,2.5)]
    progress=(t/5.3)*len(spots)
    got=min(len(spots),int(progress))
    for j,(x,z) in enumerate(spots): gem(d,x,z,t,j<got)
    path=[(0,5.5),(-4.5,4.8),(-5.2,-4.5),(-2.4,-6.3),(2.1,-4.0),(5.0,-1.2),(.4,1.2),(-4.4,2.5)]
    p=min(progress,len(path)-1)
    idx=min(int(p),len(path)-2); f=p-idx
    ax,az=path[idx]; bx,bz=path[idx+1]
    px,py=iso(ax*(1-f)+bx*f,az*(1-f)+bz*f)
    py-=14+max(0,math.sin(t*3))*9
    d.ellipse((px-16,py+10,px+16,py+20),fill=(7,13,21))
    d.ellipse((px-13,py-13,px+13,py+13),fill=(242,105,79),outline=(255,207,186),width=3)
    d.rounded_rectangle((42,126,230,196),radius=14,fill=(14,25,41),outline=(55,88,110))
    d.text((60,140),f"CRYSTALS  {got} / 6",fill=(132,243,222),font=font(18,True))
    d.text((60,169),f"TIME  {t:04.1f}s",fill=(225,234,244),font=font(15))
    d.text((54,487),"WASD move     SPACE jump     R restart",fill=(212,226,241),font=font(16))
    return im


FACES=[
    ((0,0,1),(40,190,187),[(-1,-1,1),(1,-1,1),(1,1,1),(-1,1,1)]),
    ((0,0,-1),(163,88,205),[(1,-1,-1),(-1,-1,-1),(-1,1,-1),(1,1,-1)]),
    ((1,0,0),(230,135,63),[(1,-1,1),(1,-1,-1),(1,1,-1),(1,1,1)]),
    ((-1,0,0),(69,132,205),[(-1,-1,-1),(-1,-1,1),(-1,1,1),(-1,1,-1)]),
    ((0,1,0),(225,186,70),[(-1,1,1),(1,1,1),(1,1,-1),(-1,1,-1)]),
    ((0,-1,0),(190,76,89),[(-1,-1,-1),(1,-1,-1),(1,-1,1),(-1,-1,1)]),
]


def project(point, perspective, center_x, t):
    x,y,z=point
    # Rotate around Y, then X.
    a=t*.7; b=.38
    x,z=x*math.cos(a)+z*math.sin(a),-x*math.sin(a)+z*math.cos(a)
    y,z=y*math.cos(b)-z*math.sin(b),y*math.sin(b)+z*math.cos(b)
    eye=np.array([5.,4.,7.]); target=np.zeros(3); up=np.array([0.,1.,0.])
    forward=(target-eye); forward/=np.linalg.norm(forward)
    right=np.cross(forward,up); right/=np.linalg.norm(right)
    camup=np.cross(right,forward)
    v=np.array([x,y,z])-eye
    cx=float(np.dot(v,right)); cy=float(np.dot(v,camup)); depth=float(np.dot(v,forward))
    if perspective:
        scale=300/max(.2,depth)*1.65
    else:
        scale=91
    return (center_x+cx*scale, 310-cy*scale, depth)


def draw_cube(draw, center_x, perspective, t):
    faces=[]
    for normal,color,points in FACES:
        projected=[project(p,perspective,center_x,t) for p in points]
        depth=sum(p[2] for p in projected)/4
        faces.append((depth,color,[(p[0],p[1]) for p in projected]))
    for _,color,poly in sorted(faces,key=lambda f:f[0],reverse=True):
        draw.polygon(poly,fill=color,outline=(15,27,42),width=3)


def lab10_frame(i):
    t=i/FPS
    im,d=base("LAB 10  /  PROJECTION WORKSHOP", "Same model transform and viewpoint  •  different projection")
    d.rounded_rectangle((28,112,470,467),radius=18,fill=(12,23,39),outline=(45,76,97),width=2)
    d.rounded_rectangle((490,112,932,467),radius=18,fill=(12,23,39),outline=(45,76,97),width=2)
    draw_cube(d,250,False,t); draw_cube(d,715,True,t)
    d.text((52,130),"ORTHOGRAPHIC",fill=(132,243,222),font=font(17,True))
    d.text((514,130),"PERSPECTIVE",fill=(132,243,222),font=font(17,True))
    d.text((52,435),"Parallel edges stay parallel",fill=(211,224,238),font=font(15))
    d.text((514,435),"Depth changes apparent scale",fill=(211,224,238),font=font(15))
    d.text((50,490),"glOrtho  /  constant scale",fill=(177,195,213),font=font(15))
    d.text((515,490),"gluPerspective  /  foreshortening",fill=(177,195,213),font=font(15))
    return im


def main():
    demos={"lab8":lab8_frame,"lab9":lab9_frame,"lab10":lab10_frame}
    for folder, render in demos.items():
        path=ROOT/folder/"output.mp4"
        write_mp4(path,(render(i) for i in range(FRAMES)))
        print(f"Wrote {path}")


if __name__=="__main__": main()
