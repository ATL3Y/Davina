using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TestPasserBy : MonoBehaviour {

	[SerializeField] LayerMask testMask;
	[SerializeField] float value;
	[SerializeField] float radius;
	[SerializeField] GameObject[] models;
	[SerializeField] NavMeshAgent agent;
	[SerializeField] float appearRange = 20f;

	static TestPasserBy trading;
	bool isTraded = false;
	// Use this for initialization
	void Start () {
		radius = GetComponent<CapsuleCollider> ().radius;
		Init ();
	}

	public void Init()
	{
		models [Random.Range (0, models.Length)].SetActive (true);

		float mass = Random.Range (0.7f, 1.33f);
		agent.speed *= mass;
		transform.DOMoveY ( 0.1f * mass , 0.5f / mass ).SetRelative(true).SetLoops (9999, LoopType.Yoyo).SetEase (Ease.InOutSine);

		float rand = Random.Range (0, 1f);
		float rand2 = Random.Range (-1f, 1f) * 4f ;
		if (rand < 0.25f) {
			transform.position = new Vector3 ( rand2 , 0, appearRange);
			agent.SetDestination (new Vector3 ( - rand2 , 0, -appearRange ));
		} else if (rand < 0.5f) {

			transform.position = new Vector3 ( rand2 , 0, -appearRange + rand2);
			agent.SetDestination (new Vector3 ( -rand2 , 0, appearRange + rand2));
		} else if (rand < 0.75f) {

			transform.position = new Vector3 (-appearRange , 0, rand2 );
			agent.SetDestination (new Vector3 (appearRange , 0, -rand2 ));
		} else {

			transform.position = new Vector3 (appearRange , 0, rand2 );
			agent.SetDestination (new Vector3 (-appearRange , 0, -rand2 ));
		}

		value = Random.Range (0, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 diff = transform.position - agent.destination;
		diff.y = 0;
		if ( diff.magnitude < 1f) {
			Debug.Log ("Turn Back");
			agent.SetDestination (-agent.destination);
			value = Random.Range (0, 1f);
			isTraded = false;
		}
	}

	void OnTriggerEnter( Collider col )
	{
		if ( ( ( ( 1 << col.gameObject.layer) & testMask.value ) != 0 ) && ( trading == null ) )
		{
			agent.Stop ();	
			transform.DOPause ();
//			TestLogicManager.Instance.result.text = "Press Space to trade with this person.";
			trading = this;
		}
	}

	void OnTriggerExit( Collider col )
	{
		if ( ( ( ( 1 << col.gameObject.layer) & testMask.value ) != 0 ) && (trading == this ) )
		{
			TestLogicManager.Instance.UpdateEffect (value, 0 );
			agent.Resume ();
			transform.DORestart ();
			trading = null;
			if (!isTraded) {
				TestLogicManager.Instance.result.text = "";
			}
		}
	}

	void OnTriggerStay( Collider col )
	{
		if ( ( ( 1 << col.gameObject.layer) & testMask.value ) != 0 && trading == this )
//		if ( col.gameObject.layer == testMask.value )
		{
			Vector3 toward = (Camera.main.transform.position - transform.position);
			toward.y = 0;
			float distance = toward.magnitude;
			TestLogicManager.Instance.UpdateEffect (value, Mathf.Clamp(1f - distance / radius , 0.2f , 1f ));

			if (Input.GetKeyDown (KeyCode.Space) ) {
				if (!isTraded) {
					TestLogicManager.Instance.Trade (value);
					isTraded = true;
				} else {
//					TestLogicManager.Instance.result.text = "You have been traded with this person.";
				}
			}
		}
	}
}
