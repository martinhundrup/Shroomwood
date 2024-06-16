using UnityEngine;
using System.Collections;

public class Scroller : MonoBehaviour {

    Vector3 startPosition;
    public float scrollSpeed;
    public float tileSizeZ = 1;


    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        
        float newPosition = Mathf.Sign(scrollSpeed) * Mathf.Repeat(Time.time * Mathf.Abs(scrollSpeed), Mathf.Abs(tileSizeZ));
        transform.position = startPosition + Vector3.up * newPosition;
    }
}
