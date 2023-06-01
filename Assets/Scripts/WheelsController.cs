using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class WheelsController : MonoBehaviour
{
    public CinemachineVirtualCamera CM;
    public Rigidbody rb;
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
    public float currentInput;
    public Slider policeSlider;
    public float interceptedTime;
    public bool hasBeenIntercepted;

    public float currentLapTime;
    public float lapTime;
    public float bestLapTime;
    public TMP_Text speedText;
    // Start is called before the first frame update
    void Start()
    {
        speedText = GameObject.FindGameObjectWithTag("Speed").GetComponent<TMP_Text>();
        rb = GetComponent<Rigidbody>();
        policeSlider = GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>();
        CM = GameObject.FindGameObjectWithTag("CM").GetComponent<CinemachineVirtualCamera>();
        CM.Follow = transform;
        CM.LookAt = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Respawn") > 0.1f) {
            Respawn();
        }
    }

    public void Respawn() {
        Vector3 currentRotation = transform.eulerAngles;

        currentRotation.z = 0f;
        transform.eulerAngles = currentRotation;
    }
    private void FixedUpdate() {
        if (!GameManager.sharedInstance.canStartRace) {
            return;
        }
        currentInput = Input.GetAxis("Move") * 1;
        currentAcceleration = acceleration * currentInput;
        speedText.text = ((int)(rb.velocity.magnitude * 10)).ToString();
        
        if (Input.GetAxis("Brake") < 0) {
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

        if (interceptedTime >= 5) {
            hasBeenIntercepted = true;
        }

        if (hasBeenIntercepted) {
            SceneManager.LoadScene("PoliceIntercepted");
        }
    }

    void UpdateWheel(WheelCollider col, Transform trans) {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

    private void OnCollisionStay(Collision other) {
        if (other.gameObject.CompareTag("Police")) {
            interceptedTime += 1 * Time.deltaTime;
        }
        policeSlider.value = interceptedTime;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Radar")) {
            GameManager.sharedInstance.callThePolice = true;
            canDoALap = true;
        }

        if (other.gameObject.name == "Finish" && canDoALap) {
            canDoALap = false;
            currentLap++;
            GameManager.sharedInstance.LapCompleted();

            if ( currentLap >= 4) {
                SceneManager.LoadScene("Win");
            }
        }
    }
}
