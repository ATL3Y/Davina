using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollectibleHoleSet : MonoBehaviour
{
    private List<AudioClip> audioClips = new List<AudioClip>();
    //public AudioClip holeStorySound;
    public AudioClip storySoundL;
    public AudioClip storySoundR;

    public Color Color()
    {
        return Random.ColorHSV(.5f, 1f);
    }
}
