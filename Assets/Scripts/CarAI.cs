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
    public float turnSpeed = 5;
    public float maxSpeed = 100f;
    public Material[] colors;

    [Header("Braking Options")]
    public bool isBraking = false;

    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;
    public bool isTurning = false;
    public float targetSteerAngle = 0;


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
        Sensors();
        Drive();
        ApplySteer(nodes[currentNode].position);
        Braking();
        LerpToSteerAngle();
        // Debug.Log(CalculateDistanceToCurve());
    }

    public void Sensors() {
        RaycastHit hit;
        Vector3 sensorStarPose = transform.position + frontSensorPosition;
        sensorStarPose += transform.forward * frontSensorPosition.z;
        sensorStarPose += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        isTurning = false;
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

        //Sensores derecha
        sensorStarPose += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength)) {
            if (hit.rigidbody) {
                Debug.DrawLine(sensorStarPose, hit.point);
                isTurning = true;
                avoidMultiplier -= 1f;
            }
        }

        else if (Physics.Raycast(sensorStarPose, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength)) {
            if (hit.rigidbody) {
                Debug.DrawLine(sensorStarPose, hit.point);
                isTurning = true;
                avoidMultiplier -= 0.5f;
            }
        }

        // Sensores izquierda
        sensorStarPose -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength)) {
            if (hit.rigidbody) {
                Debug.DrawLine(sensorStarPose, hit.point);
                isTurning = true;
                avoidMultiplier += 1f;
            }
        }

        else if (Physics.Raycast(sensorStarPose, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength)) {
            if (hit.rigidbody) {
                Debug.DrawLine(sensorStarPose, hit.point);
                isTurning = true;
                avoidMultiplier += 0.5f;
            }
        }

        // Sensor central
        Debug.Log(avoidMultiplier);
        if (avoidMultiplier == 0) {
            if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength * 100)) {
                if (hit.rigidbody != null) {
                    if (!hit.rigidbody.CompareTag("Racer")) {
                        isBraking = true;
                        Debug.DrawLine(sensorStarPose, hit.point);
                        isTurning = true;
                        if (hit.normal.x < 0) {
                            avoidMultiplier = -1;
                        } else {
                            avoidMultiplier = 1;
                        }
                    } else {
                        isBraking = false;
                    }
                }
            }
        }
       
        if (isTurning) {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
    }

    public void LerpToSteerAngle() {
        wheelFrontLeft.steerAngle = Mathf.Lerp(wheelFrontLeft.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFrontRight.steerAngle = Mathf.Lerp(wheelFrontRight.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
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

    public void ApplySteer(Vector3 pos) {
        if (isTurning) {
            return;
        }
        Vector3 relativeVector = transform.InverseTransformPoint(pos);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerAngle = newSteer;
    }

    public void ChangeWaypoint() {
        currentNode++;
        currentNode = (currentNode++)%nodes.Count;
    }

    public float CalculateDistanceToCurve() {
        GameObject[] turnCheckpoints = GameObject.FindGameObjectsWithTag("Turn");

        float minDIstance = float.MaxValue;

        foreach (GameObject checkpoint in turnCheckpoints) {
            Vector3 checkpointPosition = checkpoint.transform.position;

            float distance = Vector3.Distance(transform.position, checkpointPosition);

            if (distance < minDIstance) {
                minDIstance = distance;
            }
        }
        return minDIstance;
    }
    
    public void Braking() {
        if (isBraking) {
            wheelFrontRight.brakeTorque = maxBrakeTorque;
            wheelFrontLeft.brakeTorque = maxBrakeTorque;
        } else {
            wheelFrontRight.brakeTorque = 0;
            wheelFrontLeft.brakeTorque = 0;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Curva") && (currentSpeed > 50)) {
            // Debug.Log("Frena");
            // isBraking = true;
            // Braking();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Curva")) {
            isBraking = false;
        }
    }


     private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Waypoint")) {
            ChangeWaypoint();
        }

        if (other.CompareTag("Radar")) {
            GameManager.sharedInstance.callThePolice = true;
        }
    }
}
