shader_type canvas_item;
render_mode world_vertex_coords;

// === Squiggle parameters === //
group_uniforms Squiggle;
uniform vec2 scale = vec2(1.0);
uniform float strength = 1.0;
uniform float fps = 6.0;
uniform sampler2D noise : filter_linear, repeat_enable;
group_uniforms; 

varying vec4 modulate;
varying vec2 noise_uv;

// === Transparent parameters === //
group_uniforms Transparency;
uniform vec2 player_1_pos = vec2(0.5,0.5);
uniform vec2 player_2_pos = vec2(0.5,0.5);
uniform float radius = 0.5;

group_uniforms;

void vertex() {
		modulate = COLOR;

	// Use world coordinates for scale-independent squiggles, offset by position to keep pattern attached to object
	noise_uv = (VERTEX - MODEL_MATRIX[3].xy) / (vec2(textureSize(noise, 0)) * scale);
}

#define offset_multiplier vec2(PI, E)

void fragment() {
	// --- Squiggle distortion --- //
	vec2 noise_offset = vec2(floor(TIME * fps)) * offset_multiplier;
	float noise_sample = texture(noise, noise_uv + noise_offset).r * 4.0 * PI;
	vec2 direction = vec2(cos(noise_sample), sin(noise_sample));
	vec2 squiggle_uv = UV + direction * strength * 0.005;

	vec4 tex_color = texture(TEXTURE, squiggle_uv) * modulate;
	
	// --- Ocean Transparency --- //
	
	//if the distance between player pos and this pixels position is > the circles radius alpha mask = 0.0, otherwise = 1.0
	// step(a,b) == b < a ? 0.0 : 1.0
	float p1_alpha_mask = step(radius, distance(player_1_pos, UV));
	
	//do the same for p2
	float p2_alpha_mask = step(radius, distance(player_2_pos, UV));
	
	//use whichever alpha value is lower for this pixel 
	float alpha_mask = min(p1_alpha_mask, p2_alpha_mask);

	

	COLOR = tex_color;
	COLOR.a *= alpha_mask;

}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
