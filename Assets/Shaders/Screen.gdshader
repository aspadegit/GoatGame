shader_type canvas_item;
uniform vec4 pickedColor: source_color;
uniform float intensity: hint_range(0,1) = 1.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

float screen(float a, float b)
{
	
	float screenedColor =(1.0 - 2.0*(1.0-a)*(1.0-b));
	
	return (screenedColor*intensity) + ((1.0-intensity) * a);
	
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 textureColor = texture(TEXTURE, UV);
	float avgColor = (textureColor.r + textureColor.b + textureColor.g) / 3.0;
	
	float r = screen(textureColor.r, pickedColor.r);
	float g = screen(textureColor.g, pickedColor.g);
	float b = screen(textureColor.b, pickedColor.b);
	COLOR.rgba = vec4(r,g,b, pickedColor.a*textureColor.a);
}


//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
