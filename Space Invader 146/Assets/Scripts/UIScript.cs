using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour {

	private Text startLabel;

	void Start () {
		startLabel = GetComponent<Text>();
		startLabel.text = "Press Start to Continue";
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Space))
		{
			SceneManager.LoadScene ("Main");
		}
	}
}
