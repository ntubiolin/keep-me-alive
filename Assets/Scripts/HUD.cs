using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {


	public Text scoreText;
	private int score;
	// Use this for initialization
	void Start () {
		score = 0;
		UpdatePlayerScore(PlayerController.GetInstance().getPlayerLifeScore());
	}
	
	// Update is called once per frame
	void Update () {
		score++;
		UpdatePlayerScore(PlayerController.GetInstance().getPlayerLifeScore());
	}

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		// UpdateScore ();
	}

	void UpdatePlayerScore (int score)
	{
		scoreText.text = "LifeScore: " + score;
	}
}
