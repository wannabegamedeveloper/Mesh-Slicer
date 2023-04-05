using System;
using System.Collections.Generic;
using UnityEngine;

public class SliceController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform plane;
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    
    private Mesh _targetMesh;
    private Vector3[] _vertices;
    private readonly List<Vector3> _upperVertices = new List<Vector3>();
    private Vector3[] _lowerVertices;
    private Vector3 _point1Pos;
    private Vector3 _point2Pos;
    
    private void Start()
    {
        _point1Pos = point1.position;
        _point2Pos = point2.position;
    }

    private void OnDrawGizmos()
    {
        foreach (var upperVertex in _upperVertices)
        {
            Gizmos.DrawSphere(upperVertex, .1f);
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        _targetMesh = target.GetComponent<MeshFilter>().mesh;
        
        var localToWorld = transform.localToWorldMatrix;
        _vertices = _targetMesh.vertices;
        for (int i = 0; i < _targetMesh.vertices.Length; i++)
            _vertices[i] = localToWorld.MultiplyPoint3x4(_targetMesh.vertices[i]);
        
        foreach (var vertex in _vertices)
        {
            var delta = vertex - _point2Pos;
            var pointsDirection = _point1Pos - _point2Pos;

            var cross = Vector3.Cross(delta, pointsDirection);

            float dot = Vector3.Dot(cross, delta);

            print(cross);
            if (dot > 0)
                _upperVertices.Add(vertex);
        }
    }
}
