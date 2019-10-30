﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerController : MonoBehaviour {
	public event EventHandler OnDied;
	private static PlayerController instance;
	public static PlayerController GetInstance() {
		return instance;
	}
	private int lifeScores = 200;
	private bool isContinueChangeLifeScores = true;
	private float moveSpeed = 6.0f;
	private float jumpHeight = 10.0f;

	private int actualSprite = 0;
	private float spriteInterval = 0.1f;
	private Sprite[] sprites = new Sprite[2];
	private Sprite[] crouchingSprites = new Sprite[2];

	private String genomeBasePath = "Genomes/genome_";

	[SerializeField]
	private List<Cactus> cactus = new List<Cactus> ();

	[SerializeField]
	private List<Jumped> jumps = new List<Jumped>();

	private List<Genome> genomes = Utils.loadAllGenomes ();

	private bool isGrounded = true;
	private bool isCrouching = false;

	private int actualJumpGenome = 0;

	private bool isLearning = true; //If a real player is playing the game
	public int getPlayerLifeScore(){
		return lifeScores;
	}
	public bool decreaseLifeScore(int value = 1){
		lifeScores -= value;
		return true;
	}
	public void DisableContinueChangeLifeScores(){
		isContinueChangeLifeScores = false;
	}
	private static void Dino_OnDied(object sender, System.EventArgs e) {
		
		instance.DisableContinueChangeLifeScores();
	}
	// Use this for initialization
	private void Awake() {
		instance = this;
		instance.OnDied += Dino_OnDied;
	}
	void Start () {	
		GetComponent<BoxCollider2D> ().enabled = false;
		isLearning = true;
		// isLearning = (genomes.Count < 4);
		sprites = Resources.LoadAll<Sprite> ("Art/Player/Standing");
		crouchingSprites = Resources.LoadAll<Sprite> ("Art/Player/Crouching");

		GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = false;

		//Load Cactus
		foreach(GameObject c in GameObject.FindGameObjectsWithTag("cactus"))
		{			
			int cacType;
			switch (c.name) {
				case "cactus_1":
					cacType = 1;
					break;
				case "cactus_2":
					cacType = 2;
					break;
				case "cactus_3":
					cacType = 3;
					break;
				default:
					cacType = 1;
					break;
			}
			Cactus toAdd = new Cactus () {
				type = cacType,
				position = c.transform.position
			};
			cactus.Add (toAdd);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(lifeScores <= 0){
			if (OnDied != null) {
				OnDied(this, EventArgs.Empty);
			}
			// XXX 
			GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = true;
			Time.timeScale = 0;
			if(!isLearning) {
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				Time.timeScale = 1;
				Utils.actualGenome++;
			}				
		}
		spriteInterval -= Time.deltaTime;
		if(isContinueChangeLifeScores){
			decreaseLifeScore(1);
		}
		if (spriteInterval < 0) {
			spriteInterval = 0.1f;
			if (isCrouching) {				
				GetComponent<SpriteRenderer> ().sprite = crouchingSprites [actualSprite];
			} else {
				GetComponent<SpriteRenderer> ().sprite = sprites [actualSprite];
			}
			actualSprite = 1 - actualSprite;
		}

		GetComponent<Rigidbody2D> ().velocity = new Vector2 (moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);

		if (isLearning) {
			if (Input.GetKeyUp (KeyCode.DownArrow)) {
				isCrouching = false;
				GetComponent<PolygonCollider2D> ().enabled = true;
				GetComponent<BoxCollider2D> ().enabled = false;
			}
				
			if (isGrounded && (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.UpArrow))) {
				isCrouching = false;
				GetComponent<PolygonCollider2D> ().enabled = true;
				GetComponent<BoxCollider2D> ().enabled = false;

				GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, jumpHeight);
				isGrounded = false;

				Cactus c = getNextNearestCactus ();
				Jumped jump = new Jumped {
					nearestCactus = c,
					distanceToNearestCactus = c.position - GetComponent<Rigidbody2D> ().position
				};

				jumps.Add (jump);
			} else if (isGrounded && !isCrouching && Input.GetKeyDown (KeyCode.DownArrow)) {
				isCrouching = true;
				GetComponent<BoxCollider2D> ().enabled = true;
				GetComponent<PolygonCollider2D> ().enabled = false;
			}
		} else {
			if(Time.timeScale != 0) {
				if (Utils.actualGenome < genomes.Count) {
					playGenome(Utils.actualGenome);	
				}					
			}
		}
	}

	// Called when a collision happens
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.name.StartsWith ("cactus")) {			
			if (OnDied != null) OnDied(this, EventArgs.Empty);
			GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = true;
			Time.timeScale = 0;

			// Genome genome = new Genome {
			// 	fitness = Genetic.calculateFitness(jumps, cactus),
			// 	jumps = jumps
			// };

			// Utils.persistInJson (genome, genomeBasePath);

			// jumps.Clear();

			// if(!isLearning) {
			// 	SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			// 	Time.timeScale = 1;
			// 	Utils.actualGenome++;
			// }				
		} else if (coll.gameObject.name.StartsWith ("Ground")) {
			isGrounded = true;
		}else if(coll.gameObject.tag =="bread"){
			Destroy(coll.gameObject);
			lifeScores += 1000;
		}
	}

	Cactus getNextNearestCactus() {
		float nearestDist = float.PositiveInfinity;
		Cactus nearestCactus = null;
		foreach(Cactus c in cactus)
		{
			float cacX = c.position.x;
			float playerX = GetComponent<Rigidbody2D> ().position.x;
			if (cacX > playerX) {
				float dist = cacX - playerX;
				if (dist < nearestDist) {
					nearestDist = dist;
					nearestCactus = c;
				}
			}
		}

		return nearestCactus;
	}

	void playGenome(int genomeIndex) {
		float dist = getNextNearestCactus().position.x - GetComponent<Rigidbody2D> ().position.x;
		if (actualJumpGenome >= genomes[genomeIndex].jumps.Count) {
		}
		else if (dist <= genomes[genomeIndex].jumps[actualJumpGenome].distanceToNearestCactus.x && isGrounded) {
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (GetComponent<Rigidbody2D> ().velocity.x, jumpHeight);
			isGrounded = false;

			Cactus c = getNextNearestCactus ();
			Jumped jump = new Jumped {
				nearestCactus = c,
				distanceToNearestCactus = c.position - GetComponent<Rigidbody2D> ().position
			};

			jumps.Add (jump);

			actualJumpGenome++;
		}
	}
}