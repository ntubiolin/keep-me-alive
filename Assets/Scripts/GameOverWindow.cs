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
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour {

    private Text scoreText;
    private Text highscoreText;
	private static GameOverWindow instance;
	public static GameOverWindow GetInstance() {
		return instance;
	}
    private void Awake() {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();
        
        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () => { 
            Loader.Load(Loader.Scene.GameScene); 
        };
        transform.Find("retryBtn").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().ClickFunc = () => { 
            Loader.Load(Loader.Scene.MainMenu); 
        };
        transform.Find("nextBtn").GetComponent<Button_UI>().ClickFunc = () => { 
            //TODO20191221; 
            // instance.transform.Find("GameOverPic").SetActive(true);
            ShowGameOverMenu();
        };
        transform.Find("mainMenuBtn").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("nextBtn").GetComponent<Button_UI>().AddButtonSounds();

        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Start() {
        
        Player.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Retry
            Loader.Load(Loader.Scene.GameScene);
        }
    }

    private void Bird_OnDied(object sender, System.EventArgs e) {
        scoreText.text = Level.GetInstance().GetObstaclesPassedCount().ToString();

        if (Level.GetInstance().GetObstaclesPassedCount() >= Score.GetHighscore()) {
            // New Highscore!
            highscoreText.text = Score.GetHighscore() + " (NEW!)";
        } else {
            highscoreText.text = Score.GetHighscore().ToString();
        }

        Show();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
    private void HideGameOverMenu() {
        GameObject.Find("GameOverPic").SetActive(false);
        // GameObject.Find("retryBtn").SetActive(false);
        // GameObject.Find("mainMenuBtn").SetActive(false);
        // transform.Find("GameOverPic").gameObject.SetActive(false);
    }
    private void ShowGameOverMenu() {
        transform.Find("GameOverPic").gameObject.SetActive(true);
        transform.Find("retryBtn").gameObject.SetActive(true);
        transform.Find("mainMenuBtn").gameObject.SetActive(true);
        transform.Find("scoreText").gameObject.SetActive(true);
        transform.Find("highscoreText").gameObject.SetActive(true);
    }
    private void Show() {
        gameObject.SetActive(true);
    }

}
