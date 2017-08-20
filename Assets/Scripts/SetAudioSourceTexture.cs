using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAudioSourceTexture : MonoBehaviour
{
    private AudioSourceTexture alt;
    private Material mat;
    private Texture2D tex;
    private List<AudioSource> audioSources = new List<AudioSource>();
    private NiceCollectable niceColl;
    private bool coll;

    void Start ()
    {
        alt = GetComponent<AudioSourceTexture>();

        mat = GetComponent<Renderer>().material;
        tex = (Texture2D)mat.GetTexture("_EmissionMap");

        niceColl = GetComponent<NiceCollectable>();
        if (niceColl != null)
            coll = true;
        else
            coll = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        foreach (AudioSource audioSource in GetComponents<AudioSource>())
        {
            if(audioSource.isPlaying)
            {
                mat.SetTexture("_DistortTexture", alt.AudioTexture);
                if (coll)
                {
                    if (niceColl.LightSideOut)
                    {
                        mat.SetFloat("_Brightness", .95f);
                    }else
                    {
                        mat.SetFloat("_Brightness", .05f);
                    }
                }  
                //mat.SetTexture("_EmissionMap", alt.AudioTexture);
            }
        }
	}
}
