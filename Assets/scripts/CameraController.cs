using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] float downAngle;
    [HideInInspector] public float power; // Power value will be updated by ShotPowerController
    [SerializeField] GameObject cueStick;
    private float horizontalInput;
    private bool isTakingShot;
    [SerializeField] float maxDrawDistance;
    private float savedMousePos;
    [SerializeField] TextMeshProUGUI powerText;
    LineRenderer lr;

    GameManager gameManager;
    Transform cueBall;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
        {
            if (ball.GetComponent<Ball>().IsCueBall())
            {
                cueBall = ball.transform;
                break;
            }
        }
        ResetCamera();

        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (cueBall != null && !isTakingShot)
        {
            horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.RotateAround(cueBall.position, Vector3.up, horizontalInput);
        }

        Shoot();
    }

    public void ResetCamera()
    {
        cueStick.SetActive(true);
        transform.position = cueBall.position + offset;
        transform.LookAt(cueBall.position);
        transform.localEulerAngles = new Vector3(downAngle, transform.localEulerAngles.y, 0);
    }

    void Shoot()
    {
        if (gameObject.GetComponent<Camera>().enabled)
        {
            if (Input.GetButtonDown("Fire1") && !isTakingShot)
            {
                isTakingShot = true;
                savedMousePos = 0f;
            }
            else if (isTakingShot)
            {
                if (Input.GetButton("Fire1"))
                {
                    float mouseY = Input.GetAxis("Mouse Y");
                    savedMousePos += mouseY; // Note the change here to add the mouse Y movement

                    // Clamp savedMousePos between 0 and maxDrawDistance
                    savedMousePos = Mathf.Clamp(savedMousePos, 0, maxDrawDistance);

                    float powerValue = (savedMousePos / maxDrawDistance) * 100;
                    int powerValueInt = Mathf.RoundToInt(powerValue);

                    powerText.text = "Power: " + powerValueInt + "%";

                    Debug.Log("Mouse Y: " + mouseY);
                    Debug.Log("Saved Mouse: " + savedMousePos);
                    Debug.Log("Power: " + powerValue);
                    Debug.Log("Power INT: " + powerValueInt);
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    Vector3 hitDirection = transform.forward;
                    hitDirection = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;

                    // Apply force using the savedMousePos value
                    cueBall.gameObject.GetComponent<Rigidbody>().AddForce(hitDirection * power * (savedMousePos / maxDrawDistance), ForceMode.Impulse);
                    cueStick.SetActive(false);

                    gameManager.SwitchCameras();
                    isTakingShot = false;
                }
            }
        }
    }
}
