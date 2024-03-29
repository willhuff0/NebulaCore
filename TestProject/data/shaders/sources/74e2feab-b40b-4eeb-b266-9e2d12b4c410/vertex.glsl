#version 310 es

layout(location = 0) in vec3 a_pos;
layout(location = 1) in vec3 a_normal;
layout(location = 2) in vec2 a_texCoord;

layout(location = 0) out vec2 v_texCoord;
layout(location = 1) out vec3 v_worldPos;
layout(location = 2) out vec3 v_normal;

layout(location = 0) uniform mat4 nebula_matrix_viewProjection;
layout(location = 1) uniform mat4 nebula_matrix_transform;

void main() {
    v_texCoord = a_texCoord;
    v_worldPos = vec3(vec4(a_pos, 1.0) * nebula_matrix_transform);
    v_normal = a_normal * mat3(nebula_matrix_transform);

    gl_Position = nebula_matrix_viewProjection * nebula_matrix_transform * vec4(a_pos, 1);
}