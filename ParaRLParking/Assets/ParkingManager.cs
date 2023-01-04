using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    public List<ParkingSpotManager> parkingSpots;
    public ParkingSpotManager currentParkingSpot;
    public CarController carController;
    public bool RandomizeParkingSpots;

    public List<GameObject> gos;
    public List<Vector3> gosPos;
    public List<Quaternion> gosRotation;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in gos)
        {
            gosPos.Add(go.transform.position);
            gosRotation.Add(go.transform.rotation);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetMap();
        }

        
    }

    public void ResetMap()
    {
        //Selects target spot
        int ind = RandomizeParkingSpots ? Random.Range(0, parkingSpots.Count) : 0;
        currentParkingSpot = parkingSpots[ind];
        foreach (ParkingSpotManager ps in parkingSpots)
        {
            ps.gameObject.SetActive(false);
        }
        currentParkingSpot.gameObject.SetActive(true);

        //Updates car position
        carController.transform.localPosition = new Vector3(Random.Range(minX,maxX),0.5f,Random.Range(minZ,maxZ));
        carController.transform.eulerAngles = (new Vector3(0, Random.Range(0f, 360f), 0));
        carController.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        carController.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);

        //reset map elements
        for ( int i= 0; i < gos.Count;++i)
        {
            gos[i].transform.position = gosPos[i];
            gos[i].transform.rotation = gosRotation[i];
            gos[i].GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            gos[i].GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
            
        }


    }
}
