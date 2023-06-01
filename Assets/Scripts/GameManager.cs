using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;
    public bool callThePolice;
    public bool canStartRace;
    public Light currentLight;
    public Color greenLight;
    public float startTime;
    public float lapTime;
    public float bestTime;
    public TMP_Text bestTimeText;
    public TMP_Text currenLapText;
    
    // Start is called before the first frame update
    void Start()
    {
        if (sharedInstance == null) {
            sharedInstance = this;
        }
        bestTime = Mathf.Infinity;
        StartRace();
    }

    public void StartRace() {
        StartCoroutine(WaitUntilTrafficLight());
    }

    private void Update() {
        if (!canStartRace) {
            return;
        }
        lapTime += 1 * Time.deltaTime;
        if (bestTime == Mathf.Infinity) {
            bestTimeText.text = "";
        }else {
            bestTimeText.text = "Mejor tiempo: " + bestTime.ToString("0.00");
        }
        currenLapText.text = "Vuelta actual: " + lapTime.ToString("0.00");

    }

    IEnumerator WaitUntilTrafficLight() {
        yield return new WaitForSeconds(Random.Range(3,7));
        currentLight.color = greenLight;
        canStartRace = true;
        startTime = Time.time;
        lapTime = 0f;
    }

    public void LapCompleted() {
        if (lapTime < bestTime) {
            bestTime = lapTime;
        }
        lapTime = 0f;

    } 
}
