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

        transform.localPosition = LogicManager.Instance.VRRightHand.transform.position + offset;
        transform.localRotation = LogicManager.Instance.VRRightHand.transform.rotation;
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
        foreach ( MeshRenderer r in lens.outlineRenders )
        {
            r.material.SetVector ( "_OutlineColor", color );
        }
    }

    // Update is called once per frame
    void Update ( )
    {
        transform.localPosition = LogicManager.Instance.VRRightHand.transform.position + offset;
        transform.localRotation = LogicManager.Instance.VRRightHand.transform.rotation;
        transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
    }
}
