using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID
        SceneManager.LoadScene((int)SceneIndexes.Android_Main);
#else
        SceneManager.LoadScene((int)SceneIndexes.PC_Main, LoadSceneMode.Single);
#endif
    }
}

public enum SceneIndexes
{
    PC_Main = 1,
    Android_Main = 2
}
