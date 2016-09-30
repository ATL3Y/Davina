using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class BrokenColorEffect : ImageEffectBase {

	[Range(0,1)]
	[SerializeField] float toGrayRate = 0;

	[Range(0,10)]
	[SerializeField] float overflowRate = 0;

	[Range(0,1)]
	[SerializeField] float RecordThred = 0.5f;

	[Range(0,1)]
	[SerializeField] float RecordColorRate = 0;

	[Range(0,2)]
	[SerializeField] float FadeRate = 0.98f;

	[SerializeField] Texture overflowTex;

	public RenderTexture recordTex;

	// Called by camera to apply image effect
	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{ 
		if (recordTex == null) {
			recordTex = new RenderTexture (source.width, source.height,0);
			recordTex.hideFlags = HideFlags.HideAndDontSave;
		}

		material.SetFloat ("_ToGray_Rate", toGrayRate);
		material.SetFloat ("_Overflow_Rate", overflowRate);
		material.SetFloat ("_Record_Rate", RecordColorRate);
		material.SetFloat ("_Fade_Rate", FadeRate);
		material.SetTexture ("_OverflowTex", overflowTex);
		Vector3 forward = Camera.main.transform.forward;
		float alpha = Mathf.Atan (forward.z / forward.x) / Mathf.PI;
		float beta = Mathf.Atan (forward.y / Mathf.Sqrt (forward.z * forward.z + forward.x * forward.x ) ) / Mathf.PI * 2f;
		material.SetVector ("_Camera_Forward", new Vector4 (alpha, beta, forward.x, forward.y));
			

		RenderTexture overflowBuffer = RenderTexture.GetTemporary (source.width, source.height);
		RenderTexture grayBuffer = RenderTexture.GetTemporary (source.width, source.height);

		// record the color

//		material.SetTexture ("_RecordTex", recordTex);
//		material.SetFloat ("_Add_Thred", RecordThred);
//		material.SetFloat ("_Add_Rate", 1f);
//		Graphics.Blit( source ,recordTex , material , 3 );


//		material.SetTexture ("_MainTex", recordTex);
//
//		recordTex.MarkRestoreExpected ();
//
//		Graphics.Blit (recordTex, recordTex, material, 4);
//
//		Graphics.Blit (recordTex, destination );


//		material.SetTexture ("_RecordTex" , source);
//		material.SetFloat ("_Add_Thred" , 0);
//		material.SetFloat ("_Add_Rate" , RecordColorRate);
//		Graphics.Blit (recordTex, source, material, 3);
//
//		 overflow pass
		Graphics.Blit (source, destination, material, 2);
		// to gray pass
//		Graphics.Blit ( overflowBuffer, destination, material, 1);

		RenderTexture.ReleaseTemporary (overflowBuffer);
		RenderTexture.ReleaseTemporary (grayBuffer);
	}

	void OnDisable()
	{
		DestroyImmediate (recordTex);
		recordTex = null;
	}
}
