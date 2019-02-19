using UnityEngine;
using System.Collections;

public class TweenActive : MonoBehaviour {

	// Use this for initialization
	public float delayTime = 1f;
	public bool isShow = false;
	void Start () {

	}

	void OnEnable()
	{
		if (isShow)
		{
			Invoke("SetEnable", delayTime);
		}
		else
		{
			Invoke("SetDisable", delayTime);
		}

	}
	void OnDisable()
	{
		CancelInvoke("SetEnable");
		CancelInvoke("SetDisable");
	}
	public void SetDisable()
	{
		gameObject.SetActive(false);
	}
	public void SetEnable()
	{
		gameObject.SetActive(true);
	}
}
