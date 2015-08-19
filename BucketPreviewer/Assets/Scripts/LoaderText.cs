using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class LoaderText : MonoBehaviour {

	Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		text.text = "";
	}

	public void Set(string t)
	{
		text.text = t;
	}
	
}
