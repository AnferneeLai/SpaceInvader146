using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour {

	public static int lives = 3;
	public GameObject player;
	public static bool gameOver;
    public Text lifeText;
    public Image lifeImage1;
    public Image lifeImage2;

    void Start () {
        lifeText.text = "3";
        gameOver = false;
	}

	void Update () {
		if (lives == 2) {
			lifeText.text = "2";
            lifeImage2.enabled = false;
		} if (lives == 1) {
            lifeText.text = "1";
            lifeImage1.enabled = false;
        } if (lives == 0) {
			gameOver = true;
			player.SetActive (false);
            lifeText.text = "0";
		}
	}
}
