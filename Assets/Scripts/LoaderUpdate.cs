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

public class LoaderUpdate : MonoBehaviour {
    float timer;
    bool timerReached; 
    void Awake(){
        timer = 0;
        timerReached = false;
    }
    void Start()
    {
        // StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>In waiter");
        //Wait for 4 seconds
        yield return new WaitForSeconds(4);
    }
    private void Update() {
        Loader.LoadTargetScene();
        // if (!timerReached)
        // timer += Time.deltaTime;

        // if (!timerReached && timer > 5)
        // {

        //     //Set to false so that We don't run this again
        //     timerReached = true;
        // }
    }

}
