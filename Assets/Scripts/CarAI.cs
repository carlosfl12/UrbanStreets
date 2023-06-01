using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarAI : MonoBehaviour
{
    public Rigidbody rb;
    public bool canDoALap;
    public int currentLap = 1;
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
    public float fixedMaxSpeed;
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
    

    [Header("Overtake")]
    public float overtakeTime;
    public GameObject overtakePrefab;
    public bool intercepted;
    [SerializeField] private float interceptedTime;
    [SerializeField] private Vector3 policeDepartment;

    [Header("Last Position")]
    public float lastPositionTimer;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        policeDepartment = new Vector3(-53, 0.75f, -185);
        fixedMaxSpeed = maxSpeed;
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
        if (!GameManager.sharedInstance.canStartRace) {
            return;
        }
        if (gameObject.CompareTag("Police") && !GameManager.sharedInstance.callThePolice) {
            return;
        }
        if (interceptedTime >= 5f) {
            intercepted = true;
        }
        if (intercepted) {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = policeDepartment;
            return;
        }

        if (gameObject.transform.eulerAngles.z > 89 || gameObject.transform.eulerAngles.z > -89) {
            gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
        }
        Sensors();
        ChangeWaypoint();
        Drive();
        ApplySteer(nodes[currentNode].position);
        Braking();
        LerpToSteerAngle();
        overtakeTime += 1 * Time.deltaTime;
        if (overtakeTime > 1.5) {
            overtakeTime = 0;
            OvertakeSpawn();
        }

        if (Vector3.Distance(transform.position, nodes[currentNode].position) > 25) {
            CheckIfCantMove();
        }
    }

    public void CheckIfCantMove(){
        lastPositionTimer += 1 * Time.deltaTime;
        
        if (lastPositionTimer >= 10f) {
            if (currentNode - 1 <= 0) {
                transform.position = nodes[0].position;
            } else {
                transform.position = nodes[currentNode - 1].position;

            }
            lastPositionTimer = 0;
        }

    }
    public void OvertakeSpawn() {
        GameObject overtakeGO = Instantiate(overtakePrefab, new Vector3(transform.position.x , transform.position.y, transform.position.z - 3.25f), transform.rotation);
        
        Destroy(overtakeGO, 2f);
    }
    public void Sensors() {
        RaycastHit hit;
        Vector3 sensorStarPose = transform.position + frontSensorPosition;
        sensorStarPose += transform.forward * frontSensorPosition.z;
        sensorStarPose += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        isTurning = false;

        //Sensores derecha
        sensorStarPose += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength)) {
            if (hit.rigidbody) {
                Debug.DrawLine(sensorStarPose, hit.point, Color.yellow);
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
                Debug.DrawLine(sensorStarPose, hit.point, Color.red);
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
        if (avoidMultiplier == 0) {
            if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength)) {
                if (hit.rigidbody) {
                    isTurning = true;
                    if (hit.normal.x < 0) {
                        avoidMultiplier = -1;
                    } else {
                        avoidMultiplier = 1;
                    }
                }
            }
        }

        if (Physics.Raycast(sensorStarPose, transform.forward, out hit, sensorLength * 10)) {
                if ((hit.collider.gameObject.CompareTag("Building") && currentSpeed > 100)) {
                    // Debug.Log(hit.collider.name);
                    Debug.DrawLine(sensorStarPose, hit.point, Color.black);
                    isBraking = true;
                } 
            }

        if (Physics.Raycast(sensorStarPose + Vector3.up * 5, transform.forward, out hit, sensorLength * 10)) {
                if ((hit.collider.gameObject.CompareTag("Building") && currentSpeed > 100)) {
                    // Debug.Log(hit.collider.name);
                    Debug.DrawLine(sensorStarPose + Vector3.up * 5, hit.point, Color.black);
                    isBraking = true;
                } 
        }
       
       
        if((currentSpeed < maxSpeed / 2) && isBraking) {
            isBraking = false;
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
        if (Vector3.Distance(transform.position, nodes[currentNode].transform.position) < 8f) {
            currentNode++;
            lastPositionTimer = 0;
        }
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

    private void OnCollisionStay(Collision other) {
        if (other.gameObject.CompareTag("Police")) {
            interceptedTime += 1 * Time.deltaTime;
        }
    }
     private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Radar")) {
            GameManager.sharedInstance.callThePolice = true;
            canDoALap = true;
        }

        if (other.CompareTag("Overtake")) {
            maxSpeed += 50f;
        }

        if (other.gameObject.CompareTag("Finish")) {
            canDoALap = false;
            currentLap++;
            if (currentLap >= 4) {
                SceneManager.LoadScene("Lose");
            }
        }

        if (other.CompareTag("Decoration") || other.CompareTag("Building")) {
            lastPositionTimer += 1 * Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Overtake")) {
            maxSpeed = fixedMaxSpeed;
        }
    }
}
