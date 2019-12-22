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
using CodeMonkey.Utils;

public class MainMenuWindow : MonoBehaviour {
    private bool playBtnState;
    private StageManager stageManager;
    private void Awake() {
        playBtnState = false; 
        stageManager = new StageManager();
        // transform.Find("playBtn").GetComponent<Button_UI>().ClickFunc = () => { 
        //     Loader.Load(Loader.Scene.GameScene); 
        // };
        transform.Find("playBtn").GetComponent<Button_UI>().ClickFunc = () => { 
            if (playBtnState){
                Debug.Log(stageManager.GetCurrentStage());
                Player.playerType = stageManager.GetCurrentStage();
                // Player.GetInstance().loadStageSettings(stageManager.GetCurrentStage());
                Loader.Load(Loader.Scene.GameScene);
            }else{
                ChangeBackground("Turtle");
                if (!playBtnState){
                    playBtnState = !playBtnState;
                }
            }
        };
        transform.Find("stageBtnLeft").GetComponent<Button_UI>().ClickFunc = () => { 
            ChangeBackground(stageManager.GetLeftStage());
        };
        transform.Find("stageBtnRight").GetComponent<Button_UI>().ClickFunc = () => { 
            ChangeBackground(stageManager.GetRightStage());
        };
        transform.Find("playBtn").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("stageBtnLeft").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("stageBtnRight").GetComponent<Button_UI>().AddButtonSounds();
        transform.Find("quitBtn").GetComponent<Button_UI>().ClickFunc = () => { Application.Quit(); };
        transform.Find("quitBtn").GetComponent<Button_UI>().AddButtonSounds();
    }
    public void ChangeBackground(string backgroundName="play"){
        if (backgroundName == "play"){
            transform.Find("Background").gameObject.SetActive(true);
            transform.Find("StageTurtle").gameObject.SetActive(false);
            transform.Find("StageWhale").gameObject.SetActive(false);
            transform.Find("StageBird").gameObject.SetActive(false);

            transform.Find("stageBtnLeft").gameObject.SetActive(false);
            transform.Find("stageBtnRight").gameObject.SetActive(false);
        }else if(backgroundName == "Turtle"){
            transform.Find("Background").gameObject.SetActive(false);
            transform.Find("StageTurtle").gameObject.SetActive(true);
            transform.Find("StageWhale").gameObject.SetActive(false);
            transform.Find("StageBird").gameObject.SetActive(false);

            transform.Find("stageBtnLeft").gameObject.SetActive(true);
            transform.Find("stageBtnRight").gameObject.SetActive(true);
        }else if(backgroundName == "Whale"){
            transform.Find("Background").gameObject.SetActive(false);
            transform.Find("StageTurtle").gameObject.SetActive(false);
            transform.Find("StageWhale").gameObject.SetActive(true);
            transform.Find("StageBird").gameObject.SetActive(false);

            transform.Find("stageBtnLeft").gameObject.SetActive(true);
            transform.Find("stageBtnRight").gameObject.SetActive(true);
        }else if(backgroundName == "Bird"){
            transform.Find("Background").gameObject.SetActive(false);
            transform.Find("StageTurtle").gameObject.SetActive(false);
            transform.Find("StageWhale").gameObject.SetActive(false);
            transform.Find("StageBird").gameObject.SetActive(true);
            
            transform.Find("stageBtnLeft").gameObject.SetActive(true);
            transform.Find("stageBtnRight").gameObject.SetActive(true);
        }
    }
    
    public class StageManager {
        private string[] stageState;
        private int stageIdx;
        public StageManager() {
            stageState = new string[3]{
                "Turtle", 
                "Whale", 
                "Bird"
            };
            stageIdx = 0;
        }

        public string GetLeftStage() {
            if (stageIdx == 0){
                stageIdx = stageState.Length - 1;
            }else{
                stageIdx -= 1;
            }
            return stageState[stageIdx];
        }
        public string GetRightStage() {
            if (stageIdx == stageState.Length - 1){
                stageIdx = 0;
            }else{
                stageIdx += 1;
            }
            return stageState[stageIdx];
        }
        public string GetCurrentStage() {
            return stageState[stageIdx];
        }
        
    }

}
