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
    // Start is called before the first frame update
    void Start()
    {
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
        GetClosestWaypoint();
        if (GameManager.sharedInstance.callThePolice) {
            foreach (GameObject light in lights) {
                light.SetActive(true);
            }
        }
        Sensors();
        Drive();
        ApplySteer(nearestWaypoint.position);
        Braking();
        LerpToSteerAngle();
        target = GetClosestRacer();
        
    }


    void GetClosestWaypoint() {
        if (nearestWaypoint == null) {
            float maxDistance = Mathf.Infinity;
            foreach (Transform waypoint in path) {
                float distance = Vector3.Distance(transform.position, waypoint.position);
                if (distance < maxDistance) {
                    maxDistance = distance;
                    nearestWaypoint = waypoint;
                }
            }
        }
    }

    GameObject GetClosestRacer() {
        float maxDistance = Mathf.Infinity;
        foreach (GameObject racer in racers) {
            float distance = Vector3.Distance(transform.position, racer.transform.position);
            if (distance < maxDistance) {
                maxDistance = distance;
                return racer;
            }
        }
        return null;
    }
}
