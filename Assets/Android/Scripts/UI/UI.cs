#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance { get; private set; }

    [SerializeField] private GameObject DeviceButtonsParent;
    [SerializeField] private GameObject DeviceButtonPrefab;
    [SerializeField] private GameObject ConnectionPanel;
    [SerializeField] private GameObject FailurePanel;
    [SerializeField] private GameObject ModePanel;
    [SerializeField] private GameObject RouteManagementPanel;
    [SerializeField] private Button AutopilotToggle;
    [SerializeField] private Button NavigatorToggle;
    [SerializeField] private GameObject RouteDeviation;

    private List<DeviceButton> _deviceButtons = new List<DeviceButton>();
    private float _camZoomOnNewPointAdd = 3;
    private bool _routePointAdding = false;

    public int DeviceButtonsCount { get { return DeviceButtonsParent.transform.childCount; } }

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (_routePointAdding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _routePointAdding = false;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Route.instance.AddPoint(pos);
                RouteManagementPanel.SetActive(true);
                ModePanel.SetActive(true);
                Camera.main.orthographicSize /= _camZoomOnNewPointAdd;
            }
        }
    }

    public void SetRouteDeviationText(bool active)
    {
        if (RouteDeviation.activeSelf == active)
            return;
        RouteDeviation.SetActive(active);
    }
    public void SetSignalActive(bool active)
    {
        NetworkClient.instance.SendData(PacketType.C_VehicleSignal, active);
        Debug.Log(active + "=====");
    }
    public void StartAddingRoutePoint()
    {
        Camera.main.orthographicSize *= _camZoomOnNewPointAdd;
        ModePanel.SetActive(false);
        RouteManagementPanel.SetActive(false);
        _routePointAdding = true;
    }
    public void RemoveLastRoutePoint()
    {
        Route.instance.RemoveLastPoint();
    }
    public void ClearRoutePoints()
    {
        Route.instance.ClearPoints();
    }
    public void AutopilotChange(bool autopilotEnabled)
    {
        Navigator.instance.ChangeAutopilotState(autopilotEnabled);
        NavigatorToggle.interactable = autopilotEnabled;
        AutopilotToggle.interactable = !autopilotEnabled;

        if (autopilotEnabled)
        {
            Vector3[] positions = Route.instance.GetRoutePositions();
            object[] data = new object[positions.Length * 2 + 1];
            data[0] = positions.Length;
            for (int i = 1; i < positions.Length + 1; i++)
            {
                data[(i * 2 - 2) + 1] = positions[i - 1].x;
                data[(i * 2 - 1) + 1] = positions[i - 1].y;
            }
            Debug.Log("-=" + data.Length);
            NetworkClient.instance.SendData(PacketType.C_RoutePointsSend, data);
        }
        NetworkClient.instance.SendData(PacketType.C_Autopilot, autopilotEnabled);
    }
    public void SetFailurePanelVisibility(bool visible)
    {
        FailurePanel.SetActive(visible);
    }
    public void SetConnectionPanelVisibility(bool isVisible)
    {
        ConnectionPanel.SetActive(isVisible);
    }

    public void RefreshDeviceButtons(List<string> ipAdresses)
    {
        for (int i = 0; i < _deviceButtons.Count; i++)
        {
            Destroy(_deviceButtons[i].gameObject);
        }
        foreach (var adress in ipAdresses)
        {
            GameObject newBut = Instantiate(DeviceButtonPrefab, Vector3.zero, Quaternion.identity, DeviceButtonsParent.transform);
            newBut.name = adress;
            DeviceButton db = newBut.GetComponent<DeviceButton>();
            db.SetIp(adress);
            _deviceButtons.Add(db);
        }
    }

    public void RefreshBut()
    {
        Network.instance.RefreshDevices();
    }
}
#endif