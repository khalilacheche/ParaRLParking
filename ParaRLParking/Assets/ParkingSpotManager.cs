using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingSpotManager : MonoBehaviour
{
    public GameObject Car;
    public float MaxDistance = 5;
    public float xLength=3;
    public float zLength=5.17f;

    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float score = GetScore();
        updateBorders(score);
    }

    public float GetScore()
    {
        float distance = GetDistance();
        float angle = GetAngle();
        float angleDamping = Mathf.Log(1 + Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * angle)), 2);
        float distanceDamping = Mathf.Max(1 - 1/MaxDistance * distance, 0);
        float score = (float)(distanceDamping * angleDamping * System.Math.Tanh(1/(Car.GetComponent<Rigidbody>().velocity.magnitude)));
        return  score;
        
    }
    private void updateBorders(float score)
    {
        Color color= Color.Lerp(Color.red, Color.green, score);
        Vector3 Center = transform.position;
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(Center.x + xLength / 2, Center.y, Center.z + zLength / 2);
        positions[1] = new Vector3(Center.x + xLength / 2, Center.y, Center.z - zLength / 2);
        positions[2] = new Vector3(Center.x - xLength / 2, Center.y, Center.z - zLength / 2);
        positions[3] = new Vector3(Center.x - xLength / 2, Center.y, Center.z + zLength / 2);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.positionCount = 4;
        lineRenderer.SetPositions(positions);
    }
    public float GetDistance()
    {
        Vector2 targetProjVector = new Vector2(transform.position.x, transform.position.z);
        Vector2 carProjVector = new Vector2(Car.transform.position.x, Car.transform.position.z);
        
        return Vector2.Distance(targetProjVector, carProjVector);
    }
    public float GetAngle()
    {
        return Vector3.Angle(transform.forward, Car.transform.forward);
    }
}
