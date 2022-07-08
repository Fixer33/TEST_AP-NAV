#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    public static Navigator instance { get; private set; }

    public bool AutoPilotEnabled { get; private set; } = false;
    public Vector2 IndicatorPosition { get; private set; } = Vector3.zero;

    public float MapScaleMultiplier = 10f;

    [SerializeField] private float NavigatorMovementSpeed = 10f;

    private Vector3 _actualVehiclePosition;
    private bool _positionUpdated = false;
    private Coroutine _navigatorMoving;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (_positionUpdated)
            UpdateNavigatorPosition();
    }

    private void UpdateNavigatorPosition()
    {
        Vector3 normalizedPosition = new Vector3(_actualVehiclePosition.x / MapScaleMultiplier, _actualVehiclePosition.z / MapScaleMultiplier, -0.5f);
        _navigatorMoving = StartCoroutine(MoveTo(normalizedPosition));
    }
    private IEnumerator MoveTo(Vector3 newPosition)
    {
        while (transform.position != newPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, newPosition, Time.deltaTime * NavigatorMovementSpeed);
            IndicatorPosition = transform.position;
            yield return new WaitForFixedUpdate();
        }
    }
    public void UpdateVehiclePosition(Vector3 position)
    {
        _actualVehiclePosition = position;
        _positionUpdated = true;
    }
    public void ChangeAutopilotState(bool enabled)
    {
        AutoPilotEnabled = enabled;
    }
}
#endif