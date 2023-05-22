#version 310 es
precision highp float;

out vec4 FragColor;

in vec2 v_texCoord;
in vec3 v_worldPos;
in vec3 v_normal;

//layout(location = 0) uniform sampler2D nebula_texture_albedo;

void main() {
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}