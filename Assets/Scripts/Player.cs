using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

[Serializable]
public class Player : MonoBehaviour {	
	public event EventHandler OnDied;
	public event EventHandler OnStartedPlaying;
	public event EventHandler OnReStartedPlaying;
	private static Player instance;
	public static Player GetInstance() {
		return instance;
	}
	
	public static string playerType;
	
	public string standingPicPath;

	public string crouchingPicPath;
	public static string GetDebugMsg() {
		return "I am player.";
	}
	private void RunInvoke(){
		isCrouching = false;
		GetComponent<BoxCollider2D> ().enabled = false;
		GetComponent<PolygonCollider2D> ().enabled = true;
	}
	private int lifeScores = 1000;
	private bool isContinueChangeLifeScores = false;
	private float moveSpeed = 10000.0f;
	private float jumpHeight = 1000.0f;
	

	private int actualSprite = 0;
	//private int gravity;

	//public float jumpHeight;
	//public float moveSpeed;
	private float spriteInterval = 0.1f;
	private Sprite[] sprites = new Sprite[2];
	private Sprite[] crouchingSprites = new Sprite[1];

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
		// GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = true;
		Time.timeScale = 0.3f;
		instance.state = State.Dead;
		
	}
	private static void Player_OnStart(object sender, System.EventArgs e){
		instance.isContinueChangeLifeScores = true;
		Time.timeScale = 1;
		instance.state = State.Playing;
		instance.playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;// XXX Static vs dynamic? If Static, the turtle cannot jump!
		instance.Jump();
	}
	private static void Player_OnReStart(object sender, System.EventArgs e){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	// public void loadStageSettings_depreciated(string playerTypeToChange = ""){
	// 	if (playerTypeToChange != ""){
	// 		playerType = playerTypeToChange;
	// 	}
	// 	if (playerType == "Turtle"){
	// 		standingPicPath = "Art/img/turtle/walking";
	// 		crouchingPicPath = "Art/img/turtle/attack";
	// 		playerRigidbody2D.gravityScale = GameConfigs.GetInstance().turtleAttr.getGravity();
	// 	}
	// 	else if(playerType == "Bird"){
	// 		standingPicPath = "Art/img/bird/walking";
	// 		crouchingPicPath = "Art/img/bird/attack";
	// 		playerRigidbody2D.gravityScale = GameConfigs.GetInstance().birdAttr.getGravity();

	// 	}
	// 	else if(playerType == "Whale"){
	// 		standingPicPath = "Art/img/fish/walking";
	// 		crouchingPicPath = "Art/img/fish/attack";
	// 		playerRigidbody2D.gravityScale = GameConfigs.GetInstance().whaleAttr.getGravity();

	// 	}
	// 	sprites = Resources.LoadAll<Sprite> (standingPicPath);
	// 	crouchingSprites = Resources.LoadAll<Sprite> (crouchingPicPath);
	// }
	public void loadStageSettings(string playerTypeToChange = ""){
		if (playerTypeToChange != ""){
			playerType = playerTypeToChange;
		}
		GameConfigs.GetInstance().ChangeBackground(playerType);
		if (playerType == "Turtle"){
			sprites = GameConfigs.GetInstance().turtleAttr.GetSprites();
			crouchingSprites = GameConfigs.GetInstance().turtleAttr.GetCrouchingSprites();
			playerRigidbody2D.gravityScale = GameConfigs.GetInstance().turtleAttr.getGravity();
		}
		else if(playerType == "Bird"){
			sprites = GameConfigs.GetInstance().birdAttr.GetSprites();
			crouchingSprites = GameConfigs.GetInstance().birdAttr.GetCrouchingSprites();
			playerRigidbody2D.gravityScale = GameConfigs.GetInstance().birdAttr.getGravity();

		}
		else if(playerType == "Whale"){
			sprites = GameConfigs.GetInstance().whaleAttr.GetSprites();
			crouchingSprites = GameConfigs.GetInstance().whaleAttr.GetCrouchingSprites();
			playerRigidbody2D.gravityScale = GameConfigs.GetInstance().whaleAttr.getGravity();

		}
	}
	// Use this for initialization
	private void Awake() {
		// playerType = "Turtle";
		instance = this;
		playerRigidbody2D = GetComponent<Rigidbody2D>();
        playerRigidbody2D.bodyType = RigidbodyType2D.Static;// XXX What's its function?
		instance.OnDied += Dino_OnDied;
		instance.OnStartedPlaying += Player_OnStart;
		instance.OnReStartedPlaying += Player_OnReStart;
		state = State.WaitingToStart;
		loadStageSettings();
	}
	void Start () {	
		GetComponent<BoxCollider2D> ().enabled = false;// XXX What's its function
		//gravity=gameObject.GetComponent<Rigidbody2D>();
		
		// GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = false;// XXX What's its functionality? Game over scene?
		// GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = false;// XXX What's its functionality? Game over scene?
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(playerType);
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

            // Rotate player as it jumps and falls
			if (playerType == "Turtle"){
            transform.eulerAngles = new Vector3(0, 0, playerRigidbody2D.velocity.y * .15f);
				}
			else if (playerType == "Bird"){
            transform.eulerAngles = new Vector3(0, 0, playerRigidbody2D.velocity.y * .15f);
			}
			else if (playerType == "Whale"){
            transform.eulerAngles = new Vector3(0, 0, playerRigidbody2D.velocity.y * .15f);
			}
            break;
        case State.Dead:
			if(TestInput()){
				// TODO change the reload scene
				// if (OnStartedPlaying != null) {
				// 	OnReStartedPlaying(this, EventArgs.Empty);
				// 	OnStartedPlaying(this, EventArgs.Empty);
				// }
			}
            break;
        }
		if(lifeScores <= 0){
			if (OnDied != null) {
				OnDied(this, EventArgs.Empty);
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
				GetComponent<SpriteRenderer> ().sprite = crouchingSprites [0];
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
			CodeMonkey.CMDebug.TextPopupMouse("I am not afraid of trash!");
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
		if (playerType == "Turtle"){
        playerRigidbody2D.velocity = Vector2.up * GameConfigs.GetInstance().turtleAttr.getJumpAmount();
		}
		else if (playerType == "Bird"){
        playerRigidbody2D.velocity = Vector2.up * GameConfigs.GetInstance().birdAttr.getJumpAmount();
		}
		else if (playerType == "Whale"){
        playerRigidbody2D.velocity = Vector2.up * GameConfigs.GetInstance().whaleAttr.getJumpAmount();
		}
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }
	private bool TestInput() {
        return 
            Input.GetKeyDown(KeyCode.Space) || 
            Input.GetMouseButtonDown(0) ||
            Input.touchCount > 0;
    }
}