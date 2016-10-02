using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class BrokenEffect : ImageEffectBase {
	[Range(0,1)]
	[SerializeField] float ToGrayRate = 0 ;

//	[SerializeField] Texture noiseTexture;
//	[SerializeField] float noiseRate = 0;

	[SerializeField] Texture colorOverflowTexture;
	[Range(0,10)]
	[SerializeField] float colorOverflowRate = 0;


	private List<RenderTexture> renderTextureList = new List<RenderTexture>();
	// Called by camera to apply image effect
	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{

		RenderTexture grayBuffer = RenderTexture.GetTemporary (source.width, source.height);
		RenderTexture colorOverflowBuffer = RenderTexture.GetTemporary (source.width, source.height);

		// color overflow effect
		material.SetFloat("_ColorOverflowRate" , colorOverflowRate);
		material.SetTexture ("_ColorOverflowTexture", colorOverflowTexture);
		Graphics.Blit (source, colorOverflowBuffer, material, 2);

		// gray effect
		material.SetFloat ("_GrayScale", ToGrayRate);
		Graphics.Blit (colorOverflowBuffer, grayBuffer, material, 1);

		// copy to destination
		Graphics.Blit (grayBuffer, destination, material , 0);

		RenderTexture.ReleaseTemporary (grayBuffer);
		RenderTexture.ReleaseTemporary (colorOverflowBuffer);
	}
}
