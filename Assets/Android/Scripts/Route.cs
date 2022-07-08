#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    public static Route instance { get; private set; }

    [SerializeField] private LineRenderer _line;
    [SerializeField] private LineRenderer _detourLine;
    [SerializeField] private int RouteCalculatingInterval = 9;

    private Navigator _navigator;
    private bool _detourDestinationAssigned = false;

    private void Start()
    {
        instance = this;
        _navigator = FindObjectOfType<Navigator>();
    }

    private void Update()
    {
        HandleDetouring();
    }

    private void HandleDetouring()
    {
        if (Time.frameCount % RouteCalculatingInterval == 0)
        {
            bool onRoute = PointIsOnRoute(_navigator.IndicatorPosition);
            UI.instance.SetRouteDeviationText(!onRoute);
            if (onRoute)
            {
                ClearDetourPoints();
            }
            else
            {
                //Write trail
                AddDetourPositionPoint(_navigator.IndicatorPosition);

                if (_detourDestinationAssigned == false)
                {
                    _detourDestinationAssigned = true;
                    Vector3 p = GetClosestRoutePoint(_navigator.IndicatorPosition);
                    AddDetourDestinationPoint(p);
                }
                else
                {
                    Vector3 p = GetClosestRoutePoint(_navigator.IndicatorPosition);
                    if (p != _detourLine.GetPosition(_detourLine.positionCount - 1))
                    {
                        _detourLine.SetPosition(_detourLine.positionCount - 1, p);
                    }
                }
                
            }
        }
    }
    private bool PointIsOnRoute(Vector2 point)
    {
        for (int i = 0; i < _line.positionCount - 1; i++)
        {
            if (PointIsBetweenTwoOthers(point, _line.GetPosition(i), _line.GetPosition(i + 1)))
            {
                return true;
            }
        }
        return false;
    }
    private Vector3 GetClosestRoutePoint(Vector3 toPoint)
    {
        float minDistance = 999999999f;
        Vector3 closest = Vector3.zero;
        for (int i = 0; i < _line.positionCount; i++)
        {
            Vector3 p = _line.GetPosition(i);
            float dist = Vector3.Distance(toPoint, p);
            if (dist < minDistance)
            {
                closest = p;
                minDistance = dist;
            }
        }
        return closest;
    }
    private bool PointIsBetweenTwoOthers(Vector2 point, Vector2 startPoint, Vector2 endPoint)
    {
        //float dx1 = endPoint.x - startPoint.x;
        //float dy1 = endPoint.y - startPoint.y;

        //float dx = point.x - startPoint.x;
        //float dy = point.y - startPoint.y;

        //float S = dx1 * dy - dx * dy1;

        //float ab = Mathf.Sqrt(dx1 * dx1 + dy1 * dy1);

        //float  h = S / ab;
        //if (Mathf.Abs(h) < 1f / 2)
        //    return true;
        //else
        //    return false;
        float lineLength = Vector2.Distance(startPoint, endPoint);
        float distanceToStart = Vector2.Distance(point, startPoint);
        float distanceToEnd = Vector2.Distance(point, endPoint);
        bool inMinBound = distanceToStart + distanceToEnd >= lineLength - 0.1;
        bool inMaxBound = distanceToStart + distanceToEnd <= lineLength + 0.1;
        return inMaxBound && inMinBound;
    }
    private void AddDetourDestinationPoint(Vector2 pos)
    {
        _detourLine.positionCount++;
        _detourLine.SetPosition(_detourLine.positionCount - 1, pos);
    }
    private void AddDetourPositionPoint(Vector2 pos)
    {
        _detourLine.positionCount++;
        if (_detourDestinationAssigned)
        {
                _detourLine.SetPosition(_detourLine.positionCount - 1, _detourLine.GetPosition(_detourLine.positionCount - 2));
                _detourLine.SetPosition(_detourLine.positionCount - 2, pos);

        }
        else
        {
            _detourLine.SetPosition(_detourLine.positionCount - 1, pos);
        }
    }
    private void ClearDetourPoints()
    {
        _detourLine.positionCount = 0;
        _detourDestinationAssigned = false;
    }
    public Vector3[] GetRoutePositions()
    {
        Vector3[] result = new Vector3[_line.positionCount];
        for (int i = 0; i < _line.positionCount; i++)
        {
            result[i] = _line.GetPosition(i);
        }
        return result;
    }
    public void AddPoint(Vector3 pos)
    {
        _line.positionCount++;
        _line.SetPosition(_line.positionCount - 1, new Vector3(pos.x, pos.y, 0.1f));
        UI.instance.AutopilotChange(false);
    }
    public void RemoveLastPoint()
    {
        if (_line.positionCount < 0)
            return;

        _line.positionCount--;
        UI.instance.AutopilotChange(false);
    }
    public void ClearPoints()
    {
        _line.positionCount = 0;
        UI.instance.AutopilotChange(false);
    }
}
#endif