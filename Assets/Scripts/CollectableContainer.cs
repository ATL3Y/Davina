using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectableContainer : MonoBehaviour
{
    //[SerializeField] Transform parent;

    [SerializeField] GameObject prefab;
    private NiceCollectable collectable;
    [SerializeField] HoleContainer holeContainer;
    public AudioClip storySoundL;
    public AudioClip storySoundR;

    private Color color;

    [SerializeField]
    GameObject anchor;

    // Use this for initialization
    void OnEnable()
    {
        //transform.SetParent(parent);

        if(SceneManager.GetActiveScene().buildIndex == 2 )
        {
            transform.SetParent ( LogicManager.Instance.VRLeftHand.transform );
        }

        GameObject temp = GameObject.Instantiate(prefab);
        if (temp.GetComponent<NiceCollectable>() != null)
        {
            collectable = temp.GetComponent<NiceCollectable>();
        }else
        {
            Debug.Log("collectable is null");
        }

        collectable.transform.SetParent(transform);
        collectable.transform.localPosition = Vector3.zero;
        collectable.transform.localRotation = Quaternion.identity;
        //collectable.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        collectable.storySoundL = storySoundL;
        collectable.storySoundR = storySoundR;

        NiceHole hole = holeContainer.hole;
        if(hole != null)
        {
            collectable.niceHole = hole;
            foreach(FXParticlesSwirl_Parent fx in collectable.transform.GetComponentsInChildren<FXParticlesSwirl_Parent>())
            {
                fx.Hole = hole;
            }
            foreach (FXParticlesSwirl fx in collectable.transform.GetComponentsInChildren<FXParticlesSwirl>())
            {
                fx.Hole = hole;
            }
        }
        else
        {
            Debug.Log("hole is null");
        }


        collectable.Color = color;
        color = holeContainer.color;
        foreach (MeshRenderer r in collectable.outlineRenders)
        {
            r.material.SetVector("_OutlineColor", color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (anchor != null)
        {
            transform.position = anchor.transform.position + anchor.transform.forward * 2.2f - anchor.transform.up * .5f;
        }
    }
}
