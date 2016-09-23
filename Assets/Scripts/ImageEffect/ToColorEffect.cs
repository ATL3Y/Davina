using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class ToColorEffect : ImageEffectBase {
	public Color bgColor;
	public Color edgeColor;
	[Range(0,1f)]
	public float rate;
	public float sensitivityDepth = 1f;
	public float sensitivityNormals = 1f;
	public float sampleDist = 1f;



	// Called by camera to apply image effect
	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		GetComponent<Camera> ().depthTextureMode |= DepthTextureMode.DepthNormals;

		Vector2 sensitivity = new Vector2 (sensitivityDepth, sensitivityNormals);
		material.SetVector ("_Sensitivity", new Vector4 (sensitivity.x, sensitivity.y, 1.0f, sensitivity.y));
		material.SetColor ("_Color", bgColor);
		material.SetColor ("_EdgeColor", edgeColor);
		material.SetFloat ("_Rate", rate);
//		edgeDetectMaterial.SetFloat ("_BgFade", edgesOnly);
		material.SetFloat ("_SampleDistance", sampleDist);
//		edgeDetectMaterial.SetVector ("_BgColor", edgesOnlyBgColor);
//		edgeDetectMaterial.SetFloat ("_Exponent", edgeExp);
//		edgeDetectMaterial.SetFloat ("_Threshold", lumThreshold);

		Graphics.Blit (source, destination, material);
	}

}
