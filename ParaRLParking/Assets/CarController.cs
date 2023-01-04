using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float maxRPM = 3000;
    public float brakeTorque = 1000;

    private bool respawned = false;

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        if (respawned)
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                axleInfo.leftWheel.brakeTorque = 10000;
                axleInfo.rightWheel.brakeTorque = 10000;
            }
            GetComponent<Rigidbody>().isKinematic = true;
            respawned = false;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
            // (do the torque calculations here as usual)
        }
        //bool brake = Input.GetKey("space");
        //Steer(Input.GetAxis("Horizontal"));
        //Accelerate(Input.GetAxis("Vertical"));
        //Brake(brake);
        UpdateVisuals();
    }
    public void Accelerate(float value)
    {
        float motor = maxMotorTorque * value;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = Mathf.Abs(axleInfo.leftWheel.rpm) > maxRPM ? 0 : motor;
                axleInfo.rightWheel.motorTorque = Mathf.Abs(axleInfo.rightWheel.rpm) > maxRPM ? 0 : motor;
            }
        }

    }
    public void Steer(float value)
    {
        float steering = maxSteeringAngle * value;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
        }

    }
    public void Brake(int value)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.leftWheel.brakeTorque = value * brakeTorque;
            axleInfo.rightWheel.brakeTorque = value * brakeTorque;
        }
    }
    public void UpdateVisuals()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
    public void ResetWheels()
    {
        respawned = true;
    }
}