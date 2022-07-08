#if !UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance { get; private set; }

    [SerializeField] private GameObject ConnectionWaitingPanel;

    private void Start()
    {
        instance = this;
    }

    public void SetConnectionWaitingPanelVisibility(bool isVisible)
    {
        ConnectionWaitingPanel.SetActive(isVisible);
    }
}
#endif