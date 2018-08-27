using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensiones {
	public static void Limpiar(this RenderTexture rtInput) {
		RenderTexture rt = RenderTexture.active;
		RenderTexture.active = rtInput;
		GL.Clear(true, true, Color.clear);
		RenderTexture.active = rt;
	}

	public static void Limpiar(this RenderTexture rtInput, Color newColor) {
		RenderTexture rt = RenderTexture.active;
		RenderTexture.active = rtInput;
		GL.Clear(true, true, newColor);
		RenderTexture.active = rt;
	}

	public static Vector2 ToTexturePos(this Vector3 vector) {
		return (Vector2) vector / 2.5f + new Vector2(0.5f, 0.5f);
	}
}
