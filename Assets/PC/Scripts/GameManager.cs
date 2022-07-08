#if !UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] private Vehicle VehicleScript;

    private void Start()
    {
        instance = this;
    }

    public void StartGame()
    {
        UI.instance.SetConnectionWaitingPanelVisibility(false);
        VehicleScript.enabled = true;
    }

    public void StopGame()
    {
        UI.instance.SetConnectionWaitingPanelVisibility(true);
        VehicleScript.enabled = false;
    }
}
#endif