#if !UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autopilot : MonoBehaviour
{
    public static Autopilot instance { get; private set; }

    [SerializeField] private float MovementSpeed = 10f;

    public bool IsActive { get; private set; } = false;

    private Vector2[] _routePoints = new Vector2[0];
    private bool _onRoute = false;
    private Vector3 _destinationPoint;
    private int _destinationPointIndex = -1;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (IsActive && _routePoints.Length > 0)
        {
            MoveOnRoute();
        }
    }

    public void MoveOnRoute()
    {
        if (_onRoute == false)
        {
            _destinationPointIndex = GetClosestRoutePointIndex();
            Vector2 pos = _routePoints[_destinationPointIndex];
            _destinationPoint = new Vector3(pos.x, 0, pos.y);
            _onRoute = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _destinationPoint, MovementSpeed * Time.deltaTime);
            if ((transform.position.x == _destinationPoint.x) && (transform.position.z == _destinationPoint.z))
            {
                if (_destinationPointIndex + 1 >= _routePoints.Length)
                {
                    _onRoute = false;
                    IsActive = false;
                    NetworkServer.instance.SendData(PacketType.S_ManualInput);
                }
                else
                {
                    _destinationPointIndex++;
                    Vector2 pos = _routePoints[_destinationPointIndex];
                    _destinationPoint = new Vector3(pos.x, 0, pos.y);
                }
            }
        }
    }
    public void SetRoutePoints(Vector2[] points) => _routePoints = points;
    public void Activate() => SetState(true);
    public void Deactivate() => SetState(false);
    public void SetState(bool active)
    {
        IsActive = active;
        _onRoute = false;
    }
    public int GetClosestRoutePointIndex()
    {
        int result = -1;
        float minDistance = 99999999999999;
        for (int i = 0; i < _routePoints.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, _routePoints[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = i;
            }
        }
        return result;
    }
}
#endif