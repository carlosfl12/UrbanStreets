using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialScript : MonoBehaviour
{
    public Transform father;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform) {
            if (child.GetComponent<SpecialItem>().actived) {
                ActivateCanvas(child);
            } else {
                DeactivateCanvas(child);
            }
        }
    }

    public void ActivateCanvas(Transform child) {
        foreach (Transform hijo in child) {
            if (hijo.GetComponent<Canvas>()) {
                hijo.GetComponent<Canvas>().enabled = true;
            }
        }
    }
    public void DeactivateCanvas(Transform child) {
        foreach (Transform hijo in child) {
            if (hijo.GetComponent<Canvas>()) {
                hijo.GetComponent<Canvas>().enabled = false;
            }
        }
    }
}
