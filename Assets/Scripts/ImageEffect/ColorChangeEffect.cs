using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class ColorChangeEffect : ImageEffectBase {
	public Color color;
	[Range(0,1f)]
	public float rate;

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetColor ("_Color", color);
		material.SetFloat ("_Rate", rate);
		Graphics.Blit (source, destination, material);
	}
}
