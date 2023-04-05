using System;
using UnityEngine;

public class SlicerControllerPlane : MonoBehaviour
{
    private bool _startSlicing;
    private bool _entered;
    private Vector3 _endPoint;
    private SliceablePlane _hitObject;
    private bool _draw1;
    private bool _draw2;
    private Vector3 _position;
    private Vector3 _position2;
    private Transform _transform;
    
    private void Start()
    {
        _transform = transform;
    }

    private void OnDrawGizmos()
    {
        if (!_draw1) return;
        Gizmos.DrawSphere(_position, .01f);

        if (_draw2)
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawSphere(_position2, .01f);
        }
    }

    private void Update()
    {
        if (_startSlicing)
        {
            if (Camera.main != null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.transform.GetComponent<SliceablePlane>() && !_entered)
                    {
                        _hitObject = hit.transform.GetComponent<SliceablePlane>();
                        _hitObject.cutPoints[0] = hit.point;
                        _entered = true;
                        _draw1 = true;
                        _position = _hitObject.cutPoints[0];
                    }

                    _endPoint = hit.point;
                    _draw2 = true;
                    _position2 = _endPoint;
                }
                else
                {
                    if (_entered)
                    {
                        _entered = false;
                        _hitObject.cutPoints[1] = _endPoint;
                        _hitObject.Separate();
                        _hitObject = null;
                        _startSlicing = false;
                    }
                }
            }
        }
        
        if (Input.GetMouseButtonDown(0))
            _startSlicing = true;
    }
}
