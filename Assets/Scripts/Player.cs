using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.Translate(input.z * transform.forward * 10f * Time.deltaTime);
        transform.Rotate(new Vector3(0, input.x * 90 * Time.deltaTime, 0));
    }
}
