#version 330 compatibility
in vec2 vUV;
out vec4 fragColor;
uniform float uTime;
void main() {
    vec2 p = vUV - 0.5;
    float rings = 0.5 + 0.5 * sin(24.0 * length(p) - uTime * 2.5);
    float bands = 0.5 + 0.5 * sin(vUV.x * 17.0 + vUV.y * 11.0 + uTime);
    vec3 deep = vec3(0.035, 0.055, 0.18);
    vec3 cyan = vec3(0.10, 0.88, 0.88);
    vec3 violet = vec3(0.70, 0.24, 0.96);
    vec3 color = mix(deep, violet, smoothstep(0.12, 0.95, rings));
    color = mix(color, cyan, smoothstep(0.79, 0.98, bands) * 0.78);
    float glow = exp(-10.0 * dot(p, p));
    fragColor = vec4(color + glow * vec3(0.18, 0.20, 0.28), 1.0);
}
