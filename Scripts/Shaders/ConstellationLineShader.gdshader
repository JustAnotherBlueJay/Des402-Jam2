shader_type canvas_item;
render_mode world_vertex_coords;

// === Squiggle parameters ===
group_uniforms Squiggle;
uniform vec2 scale = vec2(1.0);
uniform float strength = 1.0;
uniform float fps = 6.0;
uniform sampler2D noise : filter_linear, repeat_enable;
group_uniforms;

// === Fade-in parameters ===
group_uniforms Fade;
instance uniform float progress : hint_range(0.0, 1.0) = 1.0;  // fade amount
uniform vec2 center = vec2(0.5, 0.5);                 // normalized fade center
uniform float smoothness : hint_range(0.0, 0.5) = 0.1; // softness
group_uniforms;

varying vec4 modulate;
varying vec2 noise_uv;

void vertex() {
	modulate = COLOR;
	// Use world coordinates so squiggle scale stays consistent
	noise_uv = (VERTEX - MODEL_MATRIX[3].xy) / (vec2(textureSize(noise, 0)) * scale);
}

#define offset_multiplier vec2(PI, E)

void fragment() {
	// --- Squiggle distortion ---
	vec2 noise_offset = vec2(floor(TIME * fps)) * offset_multiplier;
	float noise_sample = texture(noise, noise_uv + noise_offset).r * 4.0 * PI;
	vec2 direction = vec2(cos(noise_sample), sin(noise_sample));
	vec2 squiggle_uv = UV + direction * strength * 0.005;

	vec4 tex_color = texture(TEXTURE, squiggle_uv) * modulate;

	// --- Fade-in from center ---
	float dist = distance(UV, center);
	float alpha_mask = smoothstep(progress, progress - smoothness, dist);

	//tex_color.a *= 1.0 - alpha_mask;
	tex_color.a *= alpha_mask;

	COLOR = tex_color;
}