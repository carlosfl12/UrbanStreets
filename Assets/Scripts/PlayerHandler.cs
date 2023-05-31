using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject[] cars;
    public Material[] materials;
    // Start is called before the first frame update
    void Start()
    {
        string selectedCarName = PlayerPrefs.GetString("SelectedCar");
        string selectedColorName = PlayerPrefs.GetString("SelectedColor");

        GameObject selectedCar = null;
        Material selectedColor = null;

        foreach (GameObject car in cars) {
            if (car.name == selectedCarName) {
                selectedCar = car;
                break;
            }
        }

        foreach (Material material in materials) {
            if (material.name == selectedColorName) {
                selectedColor = material;
                break;
            }
        }

        if (selectedCar != null && selectedColor != null) {
            Instantiate(selectedCar, transform.position, transform.rotation);
            Renderer carColor = selectedCar.GetComponent<Renderer>();
            carColor.material = selectedColor;

            Debug.Log(selectedCar.name + selectedColor.name);
        } else {
            Debug.Log("No va");
            Debug.Log(PlayerPrefs.GetString("SelectedCar"));
            Debug.Log(PlayerPrefs.GetString("SelectedColor"));
        }

        PlayerPrefs.DeleteKey("SelectedCar");
        PlayerPrefs.DeleteKey("SelectedColor");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
