using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestLogicManager : MonoBehaviour {

	public TestLogicManager() { s_Instance = this; }
	public static TestLogicManager Instance { get { return s_Instance; } }
	private static TestLogicManager s_Instance;

	[SerializeField] ColorChangeEffect colorChangeEffect;
	int temMoney = 5;
	public Text result;
	[SerializeField] int requireMoney = 50;
	[SerializeField] float CreateRate = 1f;
	[SerializeField] GameObject passerByPrefab;
	[SerializeField] int maxPasserby = 30;


	public void Awake()
	{
		result.text = "Trade with the people and try to collect $" + requireMoney + " to go home.";
		StartCoroutine (CreatePasserBy ());
	}
	public void Trade(float value )
	{
		Debug.Log ("Trade");
		float rand = Random.Range (0, 1f);

		if (value < 0.125f)
			Upgrade (2);
		else if (value < 0.3)
			Upgrade (1);
		else if ( value < 0.6f )
			Upgrade (0);
		else if (value < 0.8f)
			Upgrade (-1);
		else
			Upgrade (-2);
		
	}

	public void Upgrade(int level)
	{
		Debug.Log ("Upgrad" + level);
		int gotMoney = Random.Range ( level * 5, (level + 1) * 5);
		temMoney += gotMoney;
		temMoney = Mathf.Max (0, temMoney);
		result.text = (gotMoney >= 0? "+" : "-") + "$" + Mathf.Abs(gotMoney) + "!! You now got $" + temMoney + " ($" + (requireMoney - temMoney) + " left to go home)";

		if (temMoney > requireMoney) {
			result.text = "You Can Go Home Now !";
		}
	}

	public void UpdateEffect(float value , float distanceRate )
	{
		Color toColor = Color.HSVToRGB (Mathf.Lerp (0 , 0.8f , value), 1f, 1f);
		float toRate = distanceRate;

		colorChangeEffect.color = toColor;
		colorChangeEffect.rate = toRate;


	}

	IEnumerator CreatePasserBy()
	{
		int count = 0;
		while (count < maxPasserby) {

			GameObject obj = Instantiate (passerByPrefab) as GameObject;
//			obj.GetComponent<PasserBy> ().Init ();
			count ++ ;
			yield return new WaitForSeconds (1f / CreateRate);

		}
	}


}
