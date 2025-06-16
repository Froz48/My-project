// ObjectFootprint.cs
using UnityEngine;

public class ObjectFootprint : MonoBehaviour
{
    [Tooltip("Tile size of object")]
    public Vector2Int size = new Vector2Int(1, 1);
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f); 
        Vector3 worldSize = new Vector3(size.x, size.y, 1);
        
        Vector3 centerOffset = new Vector3(size.x / 2.0f, size.y / 2.0f, 0);
        Gizmos.DrawCube(transform.position + centerOffset, worldSize);
    }
}