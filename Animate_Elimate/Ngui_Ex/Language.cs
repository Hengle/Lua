using UnityEngine;
using System.Collections;

public class Language : MonoBehaviour {

	public string keyPrefix;
	public string key;

	void Awake()
	{
		UILabel label = GetComponent<UILabel>();
		if (label != null)
		{
			label.text = StringManager.GetLocalString(keyPrefix +"_"+ key);
		}
	}
	
	
}
