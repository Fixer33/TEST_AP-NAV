#if !UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    public static Vehicle instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private float MovementSpeed = 1f;
    [SerializeField] [Range(1, 500)] private float CamRotationSpeed = 1f;
    [SerializeField] private int PositionSendInterval = 50;
    [Header("Референсы")]
    [SerializeField] private GameObject CameraPivot;
    [SerializeField] private Material NormalVehicleMaterial;
    [SerializeField] private Material SignalVehicleMaterial;
    [SerializeField] private MeshRenderer VehicleMeshRenderer;

    private bool _vehicleFailure = false;
    private bool _signalActive = false;
    private Autopilot _autopilot;
    private NetworkServer _server;

    private void Start()
    {
        instance = this;
        if (CameraPivot == null)
            throw new Exception("No CameraPivot in Vehicle");

        _autopilot = FindObjectOfType<Autopilot>();
        _server = FindObjectOfType<NetworkServer>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleCameraRotation();
        HandleOtherInput();
        UpdatePositionOnVanigator();
    }

    private void UpdatePositionOnVanigator()
    {
        if (Time.frameCount % PositionSendInterval == 0)
        {
            _server.SendData(PacketType.S_VehiclePosition, transform.position);
        }
    }
    private void HandleOtherInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _vehicleFailure = !_vehicleFailure;
            _server.SendData(PacketType.S_VehicleFailure, _vehicleFailure);
        }
    }
    private void HandleCameraRotation()
    {
        float xAxis = Input.GetAxis("Mouse X");

        CameraPivot.transform.Rotate(transform.up, xAxis * CamRotationSpeed * Time.deltaTime);
    }
    private void HandleMovement()
    {
        if (_vehicleFailure)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal == 0 && vertical == 0)
            return;

        if (_autopilot.IsActive)
        {
            _autopilot.Deactivate();
            _server.SendData(PacketType.S_ManualInput);
        }    

        Vector3 direction = transform.forward * vertical * MovementSpeed;
        direction += transform.right * horizontal * MovementSpeed;

        transform.Translate(direction * Time.deltaTime);
    }

    public void ChangeSignalState(bool active)
    {
        _signalActive = active;
        if (active)
        {
            VehicleMeshRenderer.material = SignalVehicleMaterial;
        }
        else
        {
            VehicleMeshRenderer.material = NormalVehicleMaterial;
        }
    }
}
#endif