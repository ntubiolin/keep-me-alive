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

public static class Score {

    public static void Start() {
        ResetHighscore();
        Debug.Log(Player.GetDebugMsg());
        Player.GetInstance();
        Player.GetInstance().OnDied += Score_OnDied;
    }

    private static void Score_OnDied(object sender, System.EventArgs e) {
        Debug.Log("LALALALALALALLALALALALLALA");
        TrySetNewHighscore(Level.GetInstance().GetObstaclesPassedCount());
    }

    public static int GetHighscore() {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TrySetNewHighscore(int score) {
        Debug.Log(".>>>>>>>>>>,,,,,,," + score.ToString());
        int currentHighscore = GetHighscore();
        if (score > currentHighscore) {
            // New Highscore
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;
        } else {
            return false;
        }
    }

    public static void ResetHighscore() {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}
