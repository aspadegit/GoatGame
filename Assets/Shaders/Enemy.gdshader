shader_type canvas_item;

uniform float dissolveState: hint_range(0,1) = 0.0;
uniform sampler2D dissolveNoise;
uniform int amount = 16; //pixel amount
vec4 texturePointSmooth(sampler2D smp, vec2 uv, vec2 pixel_size)
{
	vec2 ddx = dFdx(uv);
	vec2 ddy = dFdy(uv);
	vec2 lxy = sqrt(ddx * ddx + ddy * ddy);
	
	vec2 uv_pixels = uv / pixel_size;
	
	vec2 uv_pixels_floor = round(uv_pixels) - vec2(0.5f);
	vec2 uv_dxy_pixels = uv_pixels - uv_pixels_floor;
	
	uv_dxy_pixels = clamp((uv_dxy_pixels - vec2(0.5f)) * pixel_size / lxy + vec2(0.5f), 0.0f, 1.0f);
	
	uv = uv_pixels_floor * pixel_size;
	
	return textureGrad(smp, uv + uv_dxy_pixels * pixel_size, ddx, ddy);
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 textureColor = texture(TEXTURE, UV);

	vec2 grid_uv = round(UV * float(amount)) / float(amount);	//pixelates
	vec4 brightness = texture(dissolveNoise, grid_uv);
	float dissolveDif = abs(brightness.r-dissolveState);
	
	COLOR = texturePointSmooth(TEXTURE, UV, TEXTURE_PIXEL_SIZE);
	COLOR.rgb += vec3(1,0,0) * step(dissolveDif, 0.2) * step(0.01, dissolveState);
	
	COLOR.a = textureColor.a * step(dissolveState, brightness.r);
}
//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
