using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelsController : MonoBehaviour
{
    public bool canDoALap;
    public int currentLap = 1;
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    public float acceleration = 500f;
    public float breackingForce = 300f;
    public float maxTurnAngle = 15f;

    public float currentAcceleration = 0f;
    private float currentBreackForce = 0f;
    private float currentTurnAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        currentAcceleration = acceleration * Input.GetAxis("Vertical");
        
        if (Input.GetKey(KeyCode.Space)) {
            currentBreackForce = breackingForce;
        } else {
            currentBreackForce = 0f;
        }

        //Apply acceleration wheels
        frontRight.motorTorque = currentAcceleration;
        frontLeft.motorTorque = currentAcceleration;

        frontRight.brakeTorque = currentBreackForce;
        frontLeft.brakeTorque = currentBreackForce;
        backLeft.brakeTorque = currentBreackForce;
        backRight.brakeTorque = currentBreackForce;

        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);
    }

    void UpdateWheel(WheelCollider col, Transform trans) {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Radar")) {
            GameManager.sharedInstance.callThePolice = true;
            canDoALap = true;
        }

        if (other.CompareTag("Finish") && canDoALap) {
            canDoALap = false;
            currentLap++;

            if ( currentLap >= 4) {
                //Final
            }
        }
    }
}
