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
    
    private static GameConfigs instance;

    public static GameConfigs GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }


    public StageAttr turtleAttr = new StageAttr(180f, 100f, 50f);
    public StageAttr whaleAttr = new StageAttr(50f, 50f, 20f);
    public StageAttr birdAttr = new StageAttr(200f, 150f, 30f);
    public class StageAttr{
        private float JUMP_AMOUNT;
        private float PIPE_MOVE_SPEED;
        private float GRAVITY;
        public StageAttr(float jumpAmount, float pipeMoveSpeed, float gravity) {
                JUMP_AMOUNT = jumpAmount;
                PIPE_MOVE_SPEED = pipeMoveSpeed;
                GRAVITY = gravity;
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
