using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceAI : CarAI
{
    [Header("Police")]
    public GameObject[] lights;
    public Transform nearestWaypoint;
    public GameObject[] racers;
    public GameObject target;

    [Header("Distance")]
    public float distanceToWaypoint;
    public float distanceToTarget;

    // Start is called before the first frame update
    void Start()
    {
        fixedMaxSpeed = maxSpeed;
        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
        racers = GameObject.FindGameObjectsWithTag("Racer");
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.sharedInstance.callThePolice) {
            return;
        }
        GetClosestWaypoint();
        if (GameManager.sharedInstance.callThePolice) {
            foreach (GameObject light in lights) {
                light.SetActive(true);
            }
        }
        Sensors();
        target = GetClosestRacer();

        Drive();
        if (distanceToWaypoint < distanceToTarget) {
            ApplySteer(nearestWaypoint.position);
        } else {
            ApplySteer(target.transform.position);
        }
        Braking();
        LerpToSteerAngle();
        
    }


    void GetClosestWaypoint() {
        float maxDistance = Mathf.Infinity;
        foreach (Transform waypoint in path) {
            float distance = Vector3.Distance(transform.position, waypoint.position);
            if (distance < maxDistance) {
                maxDistance = distance;
                distanceToWaypoint = distance;
                nearestWaypoint = waypoint;
            }
        }
    }

    GameObject GetClosestRacer() {
        GameObject closestRacer = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject racer in racers)
        {
            float distance = Vector3.Distance(racer.transform.position, currentPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                distanceToTarget = closestDistance;
                closestRacer = racer;
            }
        }

        return closestRacer;
    }
}
