using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class CarSelection : MonoBehaviour
{
    public GameObject[] cars;
    public Material[] colors;
    public TMP_Text carNameText;
    public TMP_Text colorNameText;

    public int currentCarIndex = 0;
    public int currentColorIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        carNameText.text = cars[currentCarIndex].name;
        colorNameText.text = colors[currentColorIndex].name;
    }

    public void SelectNextCar() {
        //Para quitar el if visto en clase de Subiela
        currentCarIndex = (currentCarIndex + 1) % cars.Length;
        foreach (GameObject car in cars) {
            car.SetActive(false);
        }
        cars[currentCarIndex].SetActive(true);
        carNameText.text = cars[currentCarIndex].name;
    }

    public void SelectNextColor() {
        //Para quitar el if visto en clase de Subiela
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        cars[currentCarIndex].GetComponent<MeshRenderer>().material = colors[currentColorIndex];
        colorNameText.text = colors[currentColorIndex].name;
    }


    public void SelectPreviousCar() {
        currentCarIndex = (currentCarIndex + cars.Length - 1) % cars.Length;
        foreach (GameObject car in cars) {
            car.SetActive(false);
        }
        cars[currentCarIndex].SetActive(true);
        carNameText.text = cars[currentCarIndex].name;
    }

    public void StartGame() {
        GameObject selectedCar = cars[currentCarIndex];
        Material selectedColor = colors[currentColorIndex];

        PlayerPrefs.SetString("SelectedCar", "Player " + selectedCar.name);
        PlayerPrefs.SetString("SelectedColor", selectedColor.name);

        SceneManager.LoadScene("Game");
    }


}
