using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAudioSourceTexture : MonoBehaviour
{
    private AudioSourceTexture alt;
    private List<Material> mats = new List<Material>();
    private List<Texture2D> texs = new List<Texture2D>();
    private List<AudioSource> audioSources = new List<AudioSource>();
    private NiceCollectable niceColl;
    private bool coll;

    void Start ()
    {
        alt = GetComponent<AudioSourceTexture>();

        if(GetComponent<Renderer>() != null)
        {
            Renderer r = GetComponent<Renderer>();
            foreach (Material m in r.materials)
            {
                mats.Add(m);
                texs.Add((Texture2D)m.GetTexture("_EmissionMap"));
            }
        }

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
                for(int i = 0; i < mats.Count; i++)
                {
                    mats[i].SetTexture("_DistortTexture", alt.AudioTexture);
                    if (coll)
                    {
                        if (niceColl.LightSideOut) // Do you need this? 
                        {
                            mats[i].SetFloat("_Brightness", 1f);
                        }
                        else
                        {
                            mats[i].SetFloat("_Brightness", 1f);
                        }
                    }
                    //mat.SetTexture("_EmissionMap", alt.AudioTexture);
                }
            }
        }
	}
}
