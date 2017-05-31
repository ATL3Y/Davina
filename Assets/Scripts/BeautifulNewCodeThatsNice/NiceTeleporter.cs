using UnityEngine;
using System.Collections;

public class NiceTeleporter : Interactable 
{
	[SerializeField] Transform observeLocation;
	private GameObject player;

	[SerializeField] protected MeshRenderer[] outlineRenders;
	private Material material;
	private Color color;
	protected float outlineWidth = .2f;

	// Use this for initialization
	void Start () {
		base.Start ();

		material = new Material(Shader.Find("Outlined/Silhouette Only"));

		foreach (MeshRenderer r in outlineRenders) {
			r.material = material;
			ColorUtility.TryParseHtmlString ("#00FFFFFF", out color);
			r.material.SetFloat ("_Outline", outlineWidth);
			r.material.SetVector ("_OutlineColor", color);
		}

		player = GameObject.FindGameObjectWithTag ("Player");
		SetOutline (true);

		if (observeLocation == null)
			observeLocation = transform;
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();

		//turn off outline if you're at this umbrella 
		if (Vector3.Distance (player.transform.position, transform.position) < 2f) {
			SetOutline (false);
			return;
		} else {
			SetOutline (true);
		}

		foreach ( MeshRenderer r in outlineRenders )
		{
			r.material.SetFloat( "_Outline", m_hoverTime > 0.1f ? outlineWidth * 2.0f : outlineWidth / 2f );
		}

		m_hoverTime = Mathf.Clamp01( m_hoverTime - Time.deltaTime * 200.0f );

	}

	public override void Use (Hand hand)
	{
		//print ("trying to transport to " + gameObject.name + "by hand " + hand.gameObject.name);
		TransportManager.Instance.SetTeleporter (this);
		InputManager.Instance.FocusedObject = this; //@HACK0
		InputManager.Instance.FireTransport ();

		// make this teleporter not usable and all the others usable
		foreach (NiceTeleporter t in FindObjectsOfType<NiceTeleporter>()) 
		{
			t.useable = true;
		}
		useable = false;
	}

	public Vector3 GetObservePosition()
	{
		Vector3 dirToPlayer = player.transform.position - observeLocation.transform.position;
		dirToPlayer = dirToPlayer.normalized;
		Vector3 pos = observeLocation.transform.position + dirToPlayer * .25f;
		//print ("observe ps from passerby = " + pos);
		return pos;
	}

	public override void InUseRange()
	{
		base.InUseRange ();
		m_hoverTime = Mathf.Clamp01 (m_hoverTime + Time.deltaTime * 400.0f);
	}

	void SetOutline( bool isOn )
	{
		foreach (MeshRenderer r in outlineRenders) {
			r.enabled = isOn;
		}
	}
}
