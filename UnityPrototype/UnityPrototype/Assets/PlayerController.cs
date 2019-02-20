using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Texture2D crosshairImage;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Vector3 hookLocation;
    private bool hookPulling = false;

    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    // Movement speed in units/sec.
    public float speed = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;


    // Use this for initialization
    void Start()
    {
        // lock cursor to window
        // TODO: on pause, unlock cursor.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        TurnCamera();

        // on pressing escape
        if (Input.GetAxis("Pause") > 0)
        {
            PauseGame();
        }

        // on click, shoot grappling hook
        if (Input.GetAxis("LaunchHook") > 0)
        {
            ShootHook();
        }

        // begin reeling player in to hook location
        if (hookPulling)
        {
            PullToHook();
        }
    }

    void OnGUI()
    {
        // draw crosshair
        float xMin = (Screen.width / 2) - (crosshairImage.width / 32);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 32);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width / 16, crosshairImage.height / 16), crosshairImage);
    }

    void OnCollisionEnter(Collision collision)
    {
        StopPulling();
    }

    private void PauseGame()
    {
        Debug.Log("Pausing...");
    }

    private void PullToHook()
    {
        // Distance moved = time * speed.
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed = current distance divided by total distance.
        float fracJourney = distCovered / journeyLength;

        if (fracJourney >= 1.0f)
        {
            StopPulling();
        }

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startMarker.position, hookLocation, fracJourney);
    }

    private void ShootHook()
    {
        RaycastHit hit;
        if (Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.TransformDirection(Vector3.forward), out hit))
        {
            hookLocation = hit.point;
            hookPulling = true;
            GetComponent<Rigidbody>().useGravity = false;

            startMarker = transform;

            // Keep a note of the time the movement started.
            startTime = Time.time;

            // Calculate the journey length.
            journeyLength = Vector3.Distance(startMarker.position, hookLocation);

            print("ray hit");
        }
    }

    private void StopPulling()
    {
        print("journey over");
        hookPulling = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void TurnCamera()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
