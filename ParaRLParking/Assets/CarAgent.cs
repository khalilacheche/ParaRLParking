using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    public float MAX_EPISODE_DURATION = 30;
    public float MIN_TIME_IN_TARGET = 0.5f;
    public float MAX_NUM_COLLISIONS = 3;
    public float MAX_COLLISION_TIME = 1;
    private float remainingTime;





    private CarController carController;
    private UIManager uiManager;
    public ParkingManager parkingManager;

    public List<Radar> radars;

    private float lastDistance;
    private float lastScore;
    private int episodeCount = 0;

    private float timeInTarget;
    private float timeInCollision;

    private int collidedCount;

    public void FixedUpdate()
    {


        // End episode if timer out
        remainingTime -= Time.fixedDeltaTime;
        if (remainingTime < 0)
        {
            EndEpisode();
        }

        // add reward if getting closer
        float currentDistance = parkingManager.currentParkingSpot.GetDistance();
        if (lastDistance - currentDistance > 0.1f)
        {
            AddReward(0.001f);
            lastDistance = currentDistance;
        }


        //add reward if better score from target
        /*float currentScore = parkingManager.currentParkingSpot.GetScore();
        if(currentScore-lastScore > 0.05f)
        {
            AddReward(5*currentScore);
            lastScore = currentScore;
        }*/

    }

    //called once at the start
    public override void Initialize()
    {
        episodeCount = 0;
        uiManager = GetComponent<UIManager>();
        carController = GetComponent<CarController>();
        


    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        carController.Accelerate(actions.ContinuousActions[0]);
        carController.Steer(actions.ContinuousActions[1]);
        carController.Brake(actions.DiscreteActions[0]);
        uiManager.UpdateUI(actions.ContinuousActions[0], actions.ContinuousActions[1], actions.DiscreteActions[0], GetCumulativeReward());

        
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actionsCont = actionsOut.ContinuousActions;
        actionsCont[0] = Input.GetAxis("Vertical");
        actionsCont[1] = Input.GetAxis("Horizontal");

        var actionsDisc = actionsOut.DiscreteActions;
        actionsDisc[0] = Input.GetKey("space") ? 1 : 0;

    }
    public override void OnEpisodeBegin()
    {
        episodeCount++;
        collidedCount = 0;
        timeInCollision = 0;
        uiManager.UpdateEpisodeCount(episodeCount);
        parkingManager.ResetMap();
        //carController.ResetWheels();
        remainingTime = MAX_EPISODE_DURATION;
        lastDistance = Mathf.Infinity;
        lastScore = 0;
        timeInTarget = 0;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //add 6 radar observations

        foreach(Radar radar in radars) {
            sensor.AddObservation(radar.GetDistance());
        }



        //add position of agent
        sensor.AddObservation(transform.position.normalized);
        //Velocity of agent
        sensor.AddObservation(transform.InverseTransformVector(GetComponent<Rigidbody>().velocity).normalized);


        ParkingSpotManager target = parkingManager.currentParkingSpot;
        // add position of target
        sensor.AddObservation(transform.InverseTransformPoint(target.transform.position).normalized);

        //add angle of target

        sensor.AddObservation(target.transform.rotation.eulerAngles.y);


        //add distance

        sensor.AddObservation(target.GetDistance()) ;

        //alignment (absolute cosine of the angle between car and target)
        sensor.AddObservation(Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad*target.GetAngle())));


    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            collidedCount += 1;
            AddReward(-0.01f);
            if (collidedCount>= MAX_NUM_COLLISIONS)
            {
                EndEpisode();

            }

        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            timeInCollision += Time.fixedDeltaTime;
            if (timeInCollision > MAX_COLLISION_TIME)
            {
                AddReward(-0.01f);
                EndEpisode();
            }

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ParkingSpot"))
        {
            timeInTarget += Time.fixedDeltaTime;
            if(timeInTarget> MIN_TIME_IN_TARGET)
            {
                AddReward(0.1f + parkingManager.currentParkingSpot.GetScore());
                Debug.Log("og" + (0.1f+ parkingManager.currentParkingSpot.GetScore()));
                EndEpisode();

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ParkingSpot"))
        {
            timeInTarget = 0;
        }
    }


}
