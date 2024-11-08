shader_type canvas_item;
uniform vec4 pickedColor: source_color;

void vertex() {
	// Called for every vertex the material is visible on.
}

float decideFunction(float avg, float a, float b)
{
	float shouldMultiply = 1.0-step(0.5,avg);
	float multipliedColor = shouldMultiply* a * b;
	
	float shouldScreen = step(0.5, avg);
	float screenedColor = shouldScreen * (1.0 - 2.0*(1.0-a)*(1.0-b));
	
	return screenedColor + multipliedColor;
	
	
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 textureColor = texture(TEXTURE, UV);
	float avgColor = (textureColor.r + textureColor.b + textureColor.g) / 3.0;
	
	float r = decideFunction(avgColor, textureColor.r, pickedColor.r);
	float g = decideFunction(avgColor, textureColor.g, pickedColor.g);
	float b = decideFunction(avgColor, textureColor.b, pickedColor.b);
	COLOR.rgba = vec4(r,g,b, pickedColor.a*textureColor.a);
}


//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
