using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LensContainer : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    private Lens lens;

    public AudioClip momLight;
    public AudioClip momDark;
    public AudioClip davBeg;
    public AudioClip davEnd;

    [SerializeField] Color color;
    [SerializeField] Vector3 offset = new Vector3 ( 0.0f, 0.26f, -0.1f );

    // Use this for initialization
    void OnEnable ( )
    {
        //transform.SetParent ( LogicManager.Instance.VRRightHand.transform );

        transform.localPosition = LogicManager.Instance.VRLeftHand.transform.position + offset;
        transform.localRotation = LogicManager.Instance.VRLeftHand.transform.rotation;
        transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );

        GameObject temp = GameObject.Instantiate(prefab);
        if ( temp.GetComponent<Lens> ( ) != null )
        {
            lens = temp.GetComponent<Lens> ( );
        }
        else
        {
            Debug.Log ( "lens is null" );
        }

        lens.transform.SetParent ( transform );
        lens.transform.localPosition = Vector3.zero;
        lens.transform.localRotation = Quaternion.identity;

        lens.momSoundLight = momLight;
        lens.momSoundDark = momDark;
        lens.davSoundBeg = davBeg;
        lens.davSoundEnd = davEnd;

        lens.Color = color;
        for(int i=0; i<lens.outlineRenders.Length; i++)
        {
            Color col = (lens.outlineRenders.Length - i) * color + Color.white * i;
            col /= lens.outlineRenders.Length;
            lens.outlineRenders[i].material.SetVector( "_OutlineColor", col );
            lens.outlineRenders[i].material.SetFloat ( "_Outline", i * .05f );
            lens.outlineRenders [ i ].enabled = false;
        }
    }

    // Update is called once per frame
    void Update ( )
    {
        transform.localPosition = LogicManager.Instance.VRLeftHand.transform.position + offset;
        transform.localRotation = LogicManager.Instance.VRLeftHand.transform.rotation;
        transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
    }
}
