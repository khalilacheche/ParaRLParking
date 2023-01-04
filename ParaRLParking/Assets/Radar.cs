using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public float angle;
    public float maxDistance;

    private float distance = Mathf.Infinity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        bool hasHit = Physics.Raycast(transform.position, dir, out hit, maxDistance, layerMask);
        distance = hasHit ? hit.distance : maxDistance;
        Color color = !hasHit? Color.green : Color.Lerp(Color.red, Color.green, GetDistance());
        Debug.DrawRay(transform.position, dir * (hasHit? hit.distance : maxDistance), color);
    }
    public float GetDistance()
    {
        return distance/maxDistance;
    }

}
