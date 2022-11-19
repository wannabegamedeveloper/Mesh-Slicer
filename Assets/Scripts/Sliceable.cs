using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
    public Vector3[] cutPoints;
    [SerializeField] private GameObject quad;
    [SerializeField] private float separationForce;
    [SerializeField] private bool addRigidbody;
    
    private Vector3[] _vert;
    private Vector3[] _originalMeshVertices;
    private MeshFilter _meshFilter1;
    private MeshFilter _meshFilter2;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Separate()
    {
        var originalMesh = GetComponent<MeshFilter>().mesh;
        
        var localToWorld = transform.localToWorldMatrix;
        _originalMeshVertices = originalMesh.vertices;
        for (int i = 0; i < originalMesh.vertices.Length; i++)
            _originalMeshVertices[i] = localToWorld.MultiplyPoint3x4(originalMesh.vertices[i]);
        
        SpawnNewMesh();

        var mesh1 = new Mesh();
        var mesh2 = new Mesh();

        _vert = new Vector3[4];
        Vector2[] uv1 = new Vector2[4];
        Vector2[] uv2 = new Vector2[4];
        int[] triangles1 = new int[6];
        int[] triangles2 = new int[6];

        SetupTriangles(triangles1, triangles2);
        
        mesh1.vertices = CreateSide(0, uv1);
        mesh2.vertices = CreateSide(1, uv2);
        
        AssignMeshData(mesh1, mesh2, uv1, uv2, triangles1, triangles2);

        _meshFilter1.mesh = mesh1;
        _meshFilter2.mesh = mesh2;
        _meshFilter1.mesh.RecalculateNormals();
        _meshFilter2.mesh.RecalculateNormals();
        
        Destroy(gameObject);
        if (!addRigidbody) return;
        SeparationForce();
    }

    private void SpawnNewMesh()
    {
        var firstPart = Instantiate(quad, Vector3.zero, Quaternion.identity);
        _meshFilter1 = firstPart.GetComponent<MeshFilter>();
        
        var secondPart = Instantiate(quad, Vector3.zero, Quaternion.identity);
        _meshFilter2 = secondPart.GetComponent<MeshFilter>();
    }

    private static void SetupTriangles(IList<int> triangles1, IList<int> triangles2)
    {
        triangles1[0] = 0;
        triangles1[1] = 1;
        triangles1[2] = 2;
        
        triangles1[3] = 0;
        triangles1[4] = 2;
        triangles1[5] = 3;
        
        triangles2[0] = 2;
        triangles2[1] = 1;
        triangles2[2] = 0;
        
        triangles2[3] = 3;
        triangles2[4] = 2;
        triangles2[5] = 0;
    }

    private static void AssignMeshData(Mesh mesh1, Mesh mesh2, Vector2[] uv1, Vector2[] uv2, int[] triangles1, int[] triangles2)
    {
        mesh1.uv = uv1;
        mesh1.triangles = triangles1;
        
        mesh2.uv = uv2;
        mesh2.triangles = triangles2;
    }

    private Vector3[] CreateSide(int side, IList<Vector2> uv)
    {
        var closestVertex1 = GetClosestVertexPosition(cutPoints[0], side);
        var closestVertex2 = GetClosestVertexPosition(cutPoints[1], side);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(0, 1);

        _vert[0] = closestVertex1;
        _vert[1] = cutPoints[0];
        _vert[2] = cutPoints[1];
        _vert[3] = closestVertex2;
        
        return _vert;
    }

    private void SeparationForce()
    {
        _meshFilter1.AddComponent<Rigidbody>();
        _meshFilter2.AddComponent<Rigidbody>();
        
        _meshFilter1.AddComponent<MeshCollider>().convex = true;
        _meshFilter2.AddComponent<MeshCollider>().convex = true;
            
        var rig1 = _meshFilter1.GetComponent<Rigidbody>();
        var rig2 = _meshFilter2.GetComponent<Rigidbody>();
        
        rig1.AddForceAtPosition(Vector3.left * separationForce, cutPoints[0]);
        rig2.AddForceAtPosition(Vector3.right * separationForce, cutPoints[0]);
    }

    private Vector3 GetClosestVertexPosition(Vector3 point, int side)
    {
        var closestVertexDistance = Vector3.zero;
        float minDist = Mathf.Infinity;

        foreach (var vert in _originalMeshVertices)
        {
            if (GetVertexDirection(point, vert) != side) continue;
            float dist = Vector3.Distance(vert, point);
            
            if (!(dist < minDist)) continue;
            closestVertexDistance = vert;
            minDist = dist;
        }

        return closestVertexDistance;
    }

    private static int GetVertexDirection(Vector3 point, Vector3 target)
    {
        return point.x - target.x > target.y - point.y ? 0 : 1;
    }
}
