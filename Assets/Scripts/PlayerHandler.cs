using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject[] cars;
    public Material[] materials;
    public Renderer carRenderer;
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
            GameObject carGO = Instantiate(selectedCar, transform.position, transform.rotation);
            // Renderer carColor = selectedCar.GetComponent<Renderer>();
            carRenderer = carGO.GetComponent<Renderer>();
           
           if (carRenderer != null) {
                carRenderer.material = selectedColor;
                Debug.Log(selectedCar.name + selectedColor.name);
           } else {
            Debug.LogWarning("No hay renderer");
           }

        } else {
            Debug.Log("No va");
            Debug.Log(PlayerPrefs.GetString("SelectedCar"));
            Debug.Log(PlayerPrefs.GetString("SelectedColor"));
        }

        // PlayerPrefs.DeleteKey("SelectedCar");
        // PlayerPrefs.DeleteKey("SelectedColor");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
