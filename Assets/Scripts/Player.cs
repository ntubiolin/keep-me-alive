using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

[Serializable]
public class Player : MonoBehaviour {
	private const float JUMP_AMOUNT = 180f;
	
	public event EventHandler OnDied;
	public event EventHandler OnStartedPlaying;
	private static Player instance;
	public static Player GetInstance() {
		return instance;
	}
	private void RunInvoke(){
		isCrouching = false;
		GetComponent<BoxCollider2D> ().enabled = false;
		GetComponent<PolygonCollider2D> ().enabled = true;
	}
	private int lifeScores = 10000;
	private bool isContinueChangeLifeScores = true;
	private float moveSpeed = 1.0f;
	private float jumpHeight = 10.0f;
	

	private int actualSprite = 0;
	private float spriteInterval = 0.1f;
	private Sprite[] sprites = new Sprite[2];
	private Sprite[] crouchingSprites = new Sprite[2];

	[SerializeField]
	private List<Cactus> cactus = new List<Cactus> ();

	[SerializeField]


	private bool isGrounded = true;
	private bool isCrouching = false;

	private int actualJumpGenome = 0;

	private State state;
	private enum State {
        WaitingToStart,
        Playing,
        Dead
		
    }	
	private Rigidbody2D playerRigidbody2D;
	public int getPlayerLifeScore(){
		return lifeScores;
	}
	public void setPlayerLifeScore(int value){
		lifeScores = value;
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
		GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = true;
		Time.timeScale = 0;
	}
	// Use this for initialization
	private void Awake() {
		instance = this;
		playerRigidbody2D = GetComponent<Rigidbody2D>();
        playerRigidbody2D.bodyType = RigidbodyType2D.Static;// XXX What's its function?
		instance.OnDied += Dino_OnDied;
		state = State.WaitingToStart;
	}
	void Start () {	
		GetComponent<BoxCollider2D> ().enabled = false;// XXX What's its function
		sprites = Resources.LoadAll<Sprite> ("Art/Player/Standing");
		crouchingSprites = Resources.LoadAll<Sprite> ("Art/Player/Crouching");
		GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = false;// XXX What's its functionality?
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
        default:
        case State.WaitingToStart:
            if (TestInput()) {
                // Start playing
                state = State.Playing;
                playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;// XXX Static vs dynamic? If Static, the turtle cannot jump!
                Jump();
                if (OnStartedPlaying != null) {
					OnStartedPlaying(this, EventArgs.Empty);
				}
            }
            break;
        case State.Playing:
            if (TestInput()) {
                Jump();
            }

            // Rotate bird as it jumps and falls
            transform.eulerAngles = new Vector3(0, 0, playerRigidbody2D.velocity.y * .15f);
            break;
        case State.Dead:
            break;
        }
		if(lifeScores <= 0){
			if (OnDied != null) {
				OnDied(this, EventArgs.Empty);
			}

			if(Input.GetKeyDown (KeyCode.Space)){
				// TODO change the reload scene
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				Time.timeScale = 1;
			}
			
			
		}
		spriteInterval -= Time.deltaTime; // Time.deltaTime = 1.6xx
		if(isContinueChangeLifeScores){
			decreaseLifeScore(1);
		}
		// Walking animation
		if (spriteInterval < 0) {
			spriteInterval = 0.1f;
			if (isCrouching) {				
				GetComponent<SpriteRenderer> ().sprite = crouchingSprites [actualSprite];
			} else {
				GetComponent<SpriteRenderer> ().sprite = sprites [actualSprite];
			}
			actualSprite = 1 - actualSprite;
		}

		// if (Input.GetKeyUp (KeyCode.DownArrow)) {
		// 	isCrouching = false;
		// 	GetComponent<PolygonCollider2D> ().enabled = true;
		// 	GetComponent<BoxCollider2D> ().enabled = false;
		// }
			
		if (isGrounded && (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.UpArrow))) {
			isCrouching = false;
			GetComponent<PolygonCollider2D> ().enabled = true;
			GetComponent<BoxCollider2D> ().enabled = false;

			isGrounded = false;

		} else if (isGrounded && !isCrouching && Input.GetKeyDown (KeyCode.DownArrow)) {
			CodeMonkey.CMDebug.TextPopupMouse("FFF");
			isCrouching = true;
			GetComponent<BoxCollider2D> ().enabled = true;
			GetComponent<PolygonCollider2D> ().enabled = true;
			lifeScores = lifeScores-100;
			this.Invoke("RunInvoke",2.0f);
		}
		
	}

	// Called when a collision happens
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.name.StartsWith ("pfObstacle")) {
			if (isCrouching==false){	
				CodeMonkey.CMDebug.TextPopupMouse("Collision");		
				if (OnDied != null) OnDied(this, EventArgs.Empty);
			}
		} else if (coll.gameObject.name.StartsWith ("pfGround")) {
			isGrounded = true;
		}
	}
	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.tag =="food"){
			if (coll.gameObject != null)
				Destroy(coll.gameObject);
				SoundManager.PlaySound(SoundManager.Sound.Score);
			lifeScores += 100;
		}
	}

	private void Jump() {
        playerRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }
	private bool TestInput() {
        return 
            Input.GetKeyDown(KeyCode.Space) || 
            Input.GetMouseButtonDown(0) ||
            Input.touchCount > 0;
    }
}