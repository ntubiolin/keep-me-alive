/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigs : MonoBehaviour {
    public StageAttr turtleAttr;
    public StageAttr whaleAttr;
    public StageAttr birdAttr;
    private static GameConfigs instance;

    public static GameConfigs GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
        turtleAttr = new StageAttr(180f, 100f, 50f, 
                                                "Art/img/turtle/walking",
                                                "Art/img/turtle/attack");
        whaleAttr = new StageAttr(50f, 50f, 20f,
                                                "Art/img/fish/walking",
                                                "Art/img/fish/attack");
        birdAttr = new StageAttr(200f, 150f, 30f,
                                              "Art/img/bird/walking",
                                              "Art/img/bird/attack");
    }


    // public StageAttr turtleAttr = new StageAttr(180f, 100f, 50f, 
    //                                             "Art/img/turtle/walking",
    //                                             "Art/img/turtle/attack");
    // public StageAttr whaleAttr = new StageAttr(50f, 50f, 20f,
    //                                             "Art/img/fish/walking",
    //                                             "Art/img/fish/attack");
    // public StageAttr birdAttr = new StageAttr(200f, 150f, 30f,
    //                                           "Art/img/bird/walking",
    //                                           "Art/img/bird/attack");



    // XXX Resource.Load<Sprite> doesn't work!
    // public String EXT = "";
    // public Sprite GetMainMenuSprite(){
    //     return  Resources.Load<Sprite> ("Art/background" + EXT);
    // }
    // public Sprite GetStageSprite(String stageName){
    //     if (stageName == "turtle"){
    //         return Resources.Load<Sprite> ("Art/ui/choose_turtle" + EXT);
    //     }else if (stageName == "whale"){
    //         return Resources.Load<Sprite> ("Art/ui/choose_fish" + EXT);
    //     }else if (stageName == "bird"){
    //         return Resources.Load<Sprite> ("Art/ui/choose_bird" + EXT);
    //     }
    //     return Resources.Load<Sprite> ("Art/ui/choose_turtle" + EXT);
    // }
    public void ChangeBackground(string backgroundName="Turtle"){
        if(backgroundName == "Turtle"){
            transform.Find("TurtleStageBackground").gameObject.SetActive(true);
            transform.Find("WhaleStageBackground").gameObject.SetActive(false);
            transform.Find("BirdStageBackground").gameObject.SetActive(false);
        }else if(backgroundName == "Whale"){
            transform.Find("TurtleStageBackground").gameObject.SetActive(false);
            transform.Find("WhaleStageBackground").gameObject.SetActive(true);
            transform.Find("BirdStageBackground").gameObject.SetActive(false);
        }else if(backgroundName == "Bird"){
            transform.Find("TurtleStageBackground").gameObject.SetActive(false);
            transform.Find("WhaleStageBackground").gameObject.SetActive(false);
            transform.Find("BirdStageBackground").gameObject.SetActive(true);
        }
	}
    public class StageAttr{
        private float JUMP_AMOUNT;
        private float PIPE_MOVE_SPEED;
        private float GRAVITY;
        private string standingPicPath_; 
        private string crouchingPicPath_;
        private Sprite[] sprites; 
        private Sprite[] crouchingSprites; 
        public StageAttr(float jumpAmount, float pipeMoveSpeed, float gravity, 
                         string standingPicPath, string crouchingPicPath) {
                JUMP_AMOUNT = jumpAmount;
                PIPE_MOVE_SPEED = pipeMoveSpeed;
                GRAVITY = gravity;
                standingPicPath_ = standingPicPath;
                crouchingPicPath_ = crouchingPicPath;
                sprites = Resources.LoadAll<Sprite> (standingPicPath);
		        crouchingSprites = Resources.LoadAll<Sprite> (crouchingPicPath);
            }
        public Sprite[] GetSprites(){
            return sprites;
        }
        public Sprite[] GetCrouchingSprites(){
            return crouchingSprites;
        }
        public float getPipeMoveSpeed(){
            return PIPE_MOVE_SPEED;
        }
        public float getJumpAmount(){
            return JUMP_AMOUNT;
        }
        public float getGravity(){
            return GRAVITY;
        }
    }


}
