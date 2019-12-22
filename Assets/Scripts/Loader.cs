﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {

    public enum Scene {
        GameScene,
        Loading,
        MainMenu,
    }

    private static Scene targetScene;

    public static void Load(Scene scene) {
        SceneManager.LoadScene(Scene.Loading.ToString());
        
        // if (Scene.Loading.ToString() == "Loading"){
        // }
        targetScene = scene;
    }

    public static void LoadTargetScene() {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
