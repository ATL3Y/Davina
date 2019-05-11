using UnityEngine;
using System.Collections;

public class NiceTeleporter : Interactable 
{
	[SerializeField] Transform observeLocation;
	private GameObject player;

    [SerializeField] protected Renderer[] outlineRenders;
    [SerializeField] protected SkinnedMeshRenderer[] outlineSkinnedMeshRenders;
    public float outlineWidth = 5f;

    // Use this for initialization
    public override void Start()
    {
		base.Start ();
        /*
		foreach (Renderer r in outlineRenders)
        {
			r.material.SetFloat ("_Outline", outlineWidth);
		}

        foreach (SkinnedMeshRenderer r in outlineSkinnedMeshRenders)
        {
            r.material.SetFloat("_Outline", outlineWidth);
        }
        */

        player = GameObject.FindGameObjectWithTag ("Player");

        SetOutline (true);

		if (observeLocation == null)
			observeLocation = transform;
	}

    // Update is called once per frame
    public override void Update()
    {
		base.Update();

        // Turn off outline if you're at this umbrella 
        if (Vector3.Distance(player.transform.position, transform.position) < 2f)
        {
			SetOutline(false);
			return;
		}
        else
        {
			SetOutline(true);
		}

        foreach (Renderer r in outlineRenders)
        {
            r.material.SetFloat("_Outline", m_hoverTime > 0.1f ? outlineWidth * 2.0f : outlineWidth * 0.1f);
        }
        foreach (SkinnedMeshRenderer r in outlineSkinnedMeshRenders)
        {
            //bool state = m_hoverTime > 0.1f ? true : false;
            r.material.SetFloat("_Outline", m_hoverTime > 0.1f ? outlineWidth * 0.2f : outlineWidth * 0.01f); // characters
        }

        m_hoverTime = Mathf.Clamp01(m_hoverTime - Time.deltaTime * 200.0f);
	}

	public override void Use(Hand hand)
	{
		//print ("trying to transport to " + gameObject.name + "by hand " + hand.gameObject.name);
		TransportManager.Instance.SetTeleporter(this);

        InputManager.Instance.FocusedObject = this; //@HACK0
		InputManager.Instance.FireTransport();

		// make this teleporter not usable and all the others usable
		foreach(NiceTeleporter t in FindObjectsOfType<NiceTeleporter>()) 
		{
			t.useable = true;
		}
		useable = false;
	}

	public Vector3 GetObservePosition()
	{
		Vector3 dirToPlayer = player.transform.position - observeLocation.transform.position;
		dirToPlayer = dirToPlayer.normalized;
		Vector3 pos = observeLocation.transform.position + dirToPlayer * 1.5f;

		return observeLocation.transform.position;
	}

	public override void InUseRange()
	{
		base.InUseRange();
		m_hoverTime = Mathf.Clamp01(m_hoverTime + Time.deltaTime * 400.0f);
	}

	public void SetOutline(bool isOn)
	{
        foreach (Renderer r in outlineRenders)
        {
            r.enabled = isOn;
        }
        foreach (SkinnedMeshRenderer r in outlineSkinnedMeshRenders)
        {
            r.enabled = isOn;
        }
    }
}
