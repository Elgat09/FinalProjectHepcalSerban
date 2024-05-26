using UnityEngine;
using UnityEngine.UI;

public class ShotPowerController : MonoBehaviour
{
    public Slider powerSlider; // Reference to the UI slider
    public CameraController cameraController; // Reference to the CameraController script

    void Start()
    {
        if (powerSlider == null)
        {
            Debug.LogError("Power Slider is not set!");
            return;
        }

        if (cameraController == null)
        {
            Debug.LogError("Camera Controller is not set!");
            return;
        }

        // Set the default power value from the slider
        cameraController.power = powerSlider.value;

        // Add listener to handle changes in slider value
        powerSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        // Update the shot power in the CameraController script
        cameraController.power = value;
        Debug.Log("Slider Power Value: " + value);
    }
}
