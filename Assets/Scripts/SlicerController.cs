using UnityEngine;

public class SlicerController : MonoBehaviour
{
    private bool _startSlicing;
    private bool _entered;
    private Vector3 _endPoint;
    private Sliceable _hitObject;

    private void OnDrawGizmos()
    {
        if (_hitObject)
            Gizmos.DrawSphere(_hitObject.cutPoints[0], .1f);
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
                    if (hit.transform.GetComponent<Sliceable>() && !_entered)
                    {
                        _hitObject = hit.transform.GetComponent<Sliceable>();
                        _hitObject.cutPoints[0] = hit.point;
                        _entered = true;
                    }

                    _endPoint = hit.point;
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
