/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour {

    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 60f;
    private const float PIPE_DESTROY_X_POSITION = -100f;
    private const float PIPE_SPAWN_X_POSITION = +100f;
    private const float GROUND_DESTROY_X_POSITION = -200f;
    private const float CLOUD_DESTROY_X_POSITION = -160f;
    private const float CLOUD_SPAWN_X_POSITION = +160f;
    private const float CLOUD_SPAWN_Y_POSITION = +30f;
    private const float BIRD_X_POSITION = 0f;

    private static Level instance;

    public static Level GetInstance() {
        return instance;
    }

    private List<Transform> groundList;
    private List<Transform> cloudList;
    private float cloudSpawnTimer;
    private List<Obstacle> obstacleList;
    private List<Food> foodList;
    private int obstaclesPassedCount;
    private int foodPassedCount;
    private int obstaclesSpawned;
    private int foodSpawned;
    private float obstacleSpawnTimer = 0.5f;
    private float obstacleSpawnTimerMax;
    private float foodSpawnTimer = 0.7f;
    private float foodSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State {
        WaitingToStart,
        Playing,
        PlayerDead,
    }

    private void Awake() {
        instance = this;
        SpawnInitialGround(); 
        SpawnInitialClouds();
        obstacleList = new List<Obstacle>();
        foodList = new List<Food>();
        obstacleSpawnTimerMax = 1f;
        foodSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;

    }

    private void Start() {
        Player.GetInstance().OnDied += Player_OnDied;
        Player.GetInstance().OnStartedPlaying += Player_OnStartedPlaying;
    }

    private void Player_OnStartedPlaying(object sender, System.EventArgs e) {
        state = State.Playing;
    }

    private void Player_OnDied(object sender, System.EventArgs e) {
        state = State.PlayerDead;
    }

    private void Update() {
        if (state == State.Playing) {
            HandleTimers();
            HandleObstacleMovement();
            HandleObstacleSpawning();
            HandleFoodMovement();
            HandleFoodSpawning();
            HandleGround();
            HandleClouds();
        }
    }
    private void SpawnInitialClouds() {
        cloudList = new List<Transform>();
        Transform cloudTransform;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, CLOUD_SPAWN_Y_POSITION, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }
    private Transform GetCloudPrefabTransform() {
        switch (Random.Range(0, 3)) {
        default:
        case 0: return GameAssets.GetInstance().pfCloud_1;
        case 1: return GameAssets.GetInstance().pfCloud_2;
        case 2: return GameAssets.GetInstance().pfCloud_3;
        }
    }
    private void HandleTimers() {
        float deltaTimeFood = Random.Range(-0.3f, 0.3f);
        float deltaTimeObstacle = Random.Range(-0.2f, 0.2f);
        foodSpawnTimer += deltaTimeFood;
        obstacleSpawnTimer += deltaTimeObstacle;
    }
    private void HandleClouds() {
        // Handle Cloud Spawning
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0) {
            // Time to spawn another cloud
            float cloudSpawnTimerMax = 6f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(CLOUD_SPAWN_X_POSITION, CLOUD_SPAWN_Y_POSITION, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }

        // Handle Cloud Moving
        for (int i=0; i<cloudList.Count; i++) {
            Transform cloudTransform = cloudList[i];
            // Move cloud by less speed than pipes for Parallax
            cloudTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * .7f;

            if (cloudTransform.position.x < CLOUD_DESTROY_X_POSITION) {
                // Cloud past destroy point, destroy self
                Destroy(cloudTransform.gameObject);
                cloudList.RemoveAt(i);
                i--;
            }
        }
    }

    private void SpawnInitialGround() {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -47.5f;
        float groundWidth = 192f;
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().pfGround, new Vector3(groundWidth * 2f, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleGround() {
        foreach (Transform groundTransform in groundList) {
            groundTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

            if (groundTransform.position.x < GROUND_DESTROY_X_POSITION) {
                // Ground passed the left side, relocate on right side
                // Find right most X position
                float rightMostXPosition = -100f;
                for (int i = 0; i < groundList.Count; i++) {
                    if (groundList[i].position.x > rightMostXPosition) {
                        rightMostXPosition = groundList[i].position.x;
                    }
                }
                // Place Ground on the right most position
                float groundWidth = 192f;
                groundTransform.position = new Vector3(rightMostXPosition + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }
    private void HandleObstacleSpawning() {
        obstacleSpawnTimer -= Time.deltaTime;
        if (obstacleSpawnTimer < 0) {
            // Time to spawn another Pipe
            obstacleSpawnTimer += obstacleSpawnTimerMax;
            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            height = 1f;
            CreateGapObstacles(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }
    private void HandleFoodSpawning() {
        foodSpawnTimer -= Time.deltaTime;
        if (foodSpawnTimer < 0) {
            // Time to spawn another Pipe
            foodSpawnTimer += foodSpawnTimerMax;
            float heightEdgeLimit = 10f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            height =3f;
            CreateGapFood(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }
    private void HandleObstacleMovement() {
        for (int i=0; i<obstacleList.Count; i++) {
            Obstacle obstacle = obstacleList[i];

            bool isToTheRightOfBird = obstacle.GetXPosition() > BIRD_X_POSITION;
            obstacle.Move();
            if (isToTheRightOfBird && obstacle.GetXPosition() <= BIRD_X_POSITION) {
                // Pipe passed Bird
                obstaclesPassedCount++;
            }

            if (obstacle.GetXPosition() < PIPE_DESTROY_X_POSITION) {
                // Destroy Pipe
                obstacle.DestroySelf();
                obstacleList.Remove(obstacle);
                i--;
            }
        }
    }
    private void HandleFoodMovement() {
        HandleFoodDisappearance();

        for (int i=0; i<foodList.Count; i++) {
            Food food = foodList[i];

            bool isToTheRightOfBird = food.GetXPosition() > BIRD_X_POSITION;
            food.Move();
            if (isToTheRightOfBird && food.GetXPosition() <= BIRD_X_POSITION) {
                // Pipe passed Bird
                foodPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }

            if (food.GetXPosition() < PIPE_DESTROY_X_POSITION) {
                // Destroy Pipe
                food.DestroySelf();
                foodList.Remove(food);
                i--;
            }
        }
    }
    private void HandleFoodDisappearance() {
        for (int i=0; i<foodList.Count; i++) {
            Food food = foodList[i];
            if (food.GetTransform() == null){
                foodList.Remove(food);
                i--;
            }
        }
    }
    private void SetDifficulty(Difficulty difficulty) {
        switch (difficulty) {
        case Difficulty.Easy:
            gapSize = 50f;
            obstacleSpawnTimerMax = 3.0f;
            foodSpawnTimerMax = 3.0f;
            break;
        case Difficulty.Medium:
            gapSize = 40f;
            obstacleSpawnTimerMax = 1.3f;
            foodSpawnTimerMax = 1.3f;
            break;
        case Difficulty.Hard:
            gapSize = 33f;
            obstacleSpawnTimerMax = 1.2f;
            foodSpawnTimerMax = 1.2f;
            break;
        case Difficulty.Impossible:
            gapSize = 24f;
            obstacleSpawnTimerMax = 1.1f;
            foodSpawnTimerMax = 1.1f;
            break;
        }
    }

    private Difficulty GetDifficulty() {
        if (obstaclesSpawned >= 24) return Difficulty.Impossible;
        if (obstaclesSpawned >= 12) return Difficulty.Hard;
        if (obstaclesSpawned >= 5) return Difficulty.Medium;
        return Difficulty.Easy;
    }
    private void CreateGapFood(float gapY, float gapSize, float xPosition) {
        CreateFood(-40f, xPosition); // XXX -40f should be defined in constant
        foodSpawned++;
    }
    private void CreateGapObstacles(float gapY, float gapSize, float xPosition) {
        CreateObstacle(20f, xPosition); // XXX -40f should be defined in constant
        obstaclesSpawned++;
        SetDifficulty(GetDifficulty());
    }
    private void CreateFood(float height, float xPosition) {
        // Set up Pipe Body
        Transform food = Instantiate(GameAssets.GetInstance().pfFood);
        float foodYPosition = height;

        food.position = new Vector3(xPosition, foodYPosition);
        Food foodObj = new Food(food);
        foodList.Add(foodObj);
    }
    private void CreateObstacle(float height, float xPosition) {
        // Try obstacle with speed
        GameObject obstacle = Instantiate(GameAssets.GetInstance().pfObstacleGameObject);
        float obstacleYPosition = height;
        obstacle.GetComponent<Transform>().position = new Vector3(xPosition, obstacleYPosition);
        obstacle.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-200f, 0f), Random.Range(-200f, 0f));
        Obstacle obstacleObj = new Obstacle(obstacle);
        obstacleList.Add(obstacleObj);
        // // [Old version] Set up Pipe Body
        // Transform obstacle = Instantiate(GameAssets.GetInstance().pfObstacle);
        // float obstacleYPosition = height;

        // obstacle.position = new Vector3(xPosition, obstacleYPosition);
        // Obstacle obstacleObj = new Obstacle(obstacle);
        // obstacleList.Add(obstacleObj);
    }
    public int GetObstaclesPassedCount() {
        return obstaclesPassedCount;
    }

    /*
     * Represents a single Pipe
     * */
    
    private class Obstacle {
        private Transform obstacleTransform;
        private Rigidbody2D obstacleRigidbody2D;
        // public Obstacle(Transform obstacleTransform) {
        public Obstacle(GameObject obstacle) {
            this.obstacleTransform = obstacle.GetComponent<Transform>();
            this.obstacleRigidbody2D = obstacle.GetComponent<Rigidbody2D>();
        }
        public void Move() {
            obstacleTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }
        public float GetXPosition() {
            return obstacleTransform.position.x;
        }
        public void DestroySelf() {
            Destroy(obstacleTransform.gameObject);
            Destroy(obstacleRigidbody2D.gameObject);
        }
    }
    public class Food {

        private Transform obstacleTransform;

        public Food(Transform obstacleTransform) {
            // XXX what does this public function mean??? constructor function?
            this.obstacleTransform = obstacleTransform;
        }

        public void Move() {
            obstacleTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition() {
            return obstacleTransform.position.x;
        }

        public void DestroySelf() {
            Destroy(obstacleTransform.gameObject);
        }
        public Transform GetTransform() {
            return obstacleTransform;
        }
    }

}

