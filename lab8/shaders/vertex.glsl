#version 330 compatibility
layout(location = 0) in vec2 aPosition;
layout(location = 1) in vec2 aUV;
out vec2 vUV;
uniform float uTime;
uniform float uAspect;
void main() {
    float wave = sin(aPosition.x * 5.0 + uTime * 1.7) * 0.10;
    vec2 p = aPosition + vec2(0.0, wave * (0.25 + aUV.y));
    p.x /= uAspect;
    vUV = aUV;
    gl_Position = vec4(p, 0.0, 1.0);
}
