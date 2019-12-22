﻿/* 
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

public class GameAssets : MonoBehaviour {

    private static GameAssets instance;

    public static GameAssets GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }


    public Sprite pipeHeadSprite;
    public Transform pfObstacle;
    public GameObject pfObstacleGameObject;
    public GameObject[] pfObstaclesTurtle;
    public GameObject[] pfObstaclesWhale;
    public GameObject[] pfObstaclesBird;
    public Transform pfFood;
    public Transform pfFoodTurtle;
    public Transform pfFoodWhale;
    public Transform pfFoodBird;
    public Transform pfPipeHead;
    public Transform pfPipeBody;
    public Transform pfGround;
    public Transform pfCloud_1;
    public Transform pfCloud_2;
    public Transform pfCloud_3;
    public Transform pfMainMenuBackground;
    public Transform pfStageTurtle;
    public Transform pfStageWhale;
    public Transform pfStageBird;

    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }


}
