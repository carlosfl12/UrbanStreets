using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;
    public bool callThePolice;
    // Start is called before the first frame update
    void Start()
    {
        if (sharedInstance == null) {
            sharedInstance = this;
        }
    }
}
