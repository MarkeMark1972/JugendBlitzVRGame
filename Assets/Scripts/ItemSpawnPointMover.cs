using UnityEngine;

public class ItemSpawnPointMover : MonoBehaviour
{
    public float speed = 2f;
    public float maxOffset = 5f;

    private Vector3 startPosition;
    
    private void Start()
    {
        startPosition = this.transform.position;
    }

    private void Update()
    {
        var x = Mathf.Sin(Time.time * speed) * maxOffset;
        this.transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z + x);
    }
}