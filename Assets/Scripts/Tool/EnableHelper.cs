using UnityEngine;
using System.Collections;

public class EnableHelper : MonoBehaviour {

	[SerializeField]GameObject Target;

	public void EnableTarget()
	{
		Target.SetActive (true);
	}
	public void DisableTarget()
	{
		Target.SetActive (false);
	}
}
