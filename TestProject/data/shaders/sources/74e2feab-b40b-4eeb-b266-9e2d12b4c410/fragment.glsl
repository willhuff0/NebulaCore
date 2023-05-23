#version 310 es
precision highp float;

out vec4 FragColor;

in vec2 v_texCoord;
in vec3 v_worldPos;
in vec3 v_normal;

layout(binding = 0) uniform sampler2D nebula_texture_albedo;
layout(binding = 1) uniform sampler2D nebula_texture_metallicRoughnessOcclusion;
layout(binding = 2) uniform sampler2D nebula_texture_normal;

layout(location = 2) uniform vec3 nebula_worldViewPos;

const float PI = 3.14159265359;

//------------------
//  Specular

// GGX NDF
float specular_distribution(float NoH, float roughness) {
    float a = NoH * roughness;
    float k = roughness / (1.0 - NoH * NoH + a * a);
    return k * k * (1.0 / PI);
}

// Smith GGX geometric shadowing
float specular_visibility(float NoV, float NoL, float roughness) {
    float a2 = roughness * roughness;
    float GGXV = NoL * sqrt(NoV * NoV * (1.0 - a2) + a2);
    float GGXL = NoV * sqrt(NoL * NoL * (1.0 - a2) + a2);
    return 0.5 / (GGXV + GGXL);
}

// Schlick Cook-Torrance approximation fresnel
vec3 specular_fresnel(float u, vec3 f0) {
    float f = pow(1.0 - u, 5.0);
    return f + f0 * (1.0 - f);
}

//------------------


//------------------
//  Diffuse

// Lambert diffuse
float diffuse_lambert() {
    return 1.0 / PI;
}

//------------------

void main() {
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}