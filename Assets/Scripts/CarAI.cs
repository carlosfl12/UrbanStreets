using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public bool isOvertaking = false;
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public List<Transform> nodes;
    public int currentNode = 0;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 150f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public bool isBraking = false;
    public bool isTurning = false;
    public Material[] colors;

    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;

    private void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        mesh.material = colors[Random.Range(0, colors.Length)];
        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
    }

    private void FixedUpdate() {
        Drive();
        if (!isOvertaking) {
            ApplySteer(nodes[currentNode].position);
        }
        ChangeWaypoint();
        Braking();
        Sensors();
    }

    public void Sensors() {
        RaycastHit hit;
        Vector3 sensorStarPose = transform.position + frontSensorPosition;
        sensorStarPose.z += sensorLength;
        
        // Sensor central
        if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength)) {
            Debug.DrawLine(sensorStarPose, hit.point);
            Debug.Log(hit.rigidbody);

            if (hit.rigidbody) {
                Turn(hit.rigidbody.position + new Vector3(10f, 0f, 0f));
            }
        }

        //Sensores derecha
        sensorStarPose.x += frontSideSensorPosition;
        if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength)) {
            Debug.DrawLine(sensorStarPose, hit.point);

            if (hit.rigidbody && !isTurning) {
                
            }
        }

        if (Physics.Raycast(sensorStarPose, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength)) {
            Debug.DrawLine(sensorStarPose, hit.point);
            if (hit.rigidbody) {
                StopTurning();
            }
        }

        // Sensores izquierda
        sensorStarPose.x -= 2 * frontSideSensorPosition;
        if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength)) {
            Debug.DrawLine(sensorStarPose, hit.point);

            if (hit.rigidbody && !isTurning) {
                // Turn(hit.rigidbody.position + new Vector3(-10f, 0f, 0f));
            }
        }

        if (Physics.Raycast(sensorStarPose, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength)) {
            Debug.DrawLine(sensorStarPose, hit.point);
            if (hit.rigidbody) {
                StopTurning();
            }
        }
    }

    public void Turn(Vector3 pos) {
        Debug.Log("Deberia girar derecha o izquierda");
        isOvertaking = true;
        isTurning = true;
        ApplySteer(pos);

    }

    public void StopTurning() {
        Debug.Log("Deberia dejar de girar");
        isOvertaking = false;
        isTurning = false;
    }

    public void Drive() {
        currentSpeed = 2 * Mathf.PI * wheelFrontLeft.radius * wheelFrontLeft.rpm * 60 /100;
        if (currentSpeed < maxSpeed && !isBraking) {
            wheelFrontLeft.motorTorque = maxMotorTorque;
            wheelFrontRight.motorTorque = maxMotorTorque;
        } else {
            wheelFrontLeft.motorTorque = 0;
            wheelFrontRight.motorTorque = 0;
        }
    }

    void ApplySteer(Vector3 pos) {
        // Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        Vector3 relativeVector = transform.InverseTransformPoint(pos);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFrontLeft.steerAngle = newSteer;
        wheelFrontRight.steerAngle = newSteer;
    }

    void ChangeWaypoint() {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 3f) {
            currentNode++;
        }
        currentNode = (currentNode++)%nodes.Count;
    }
    
    void Braking() {
        if (isBraking) {
            wheelFrontRight.brakeTorque = maxBrakeTorque;
            wheelFrontLeft.brakeTorque = maxBrakeTorque;
        } else {
            wheelFrontRight.brakeTorque = 0;
            wheelFrontLeft.brakeTorque = 0;
        }
    }
}
