using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Texture2D crosshairImage;
    private Rigidbody body;

    public float timeRemaining;
    private int prevRoundedTime;
    private int roundedTime;
    private int timeForUI;

    public float moveSpeed;

    public float mouseSpeedH = 2.0f;
    public float mouseSpeedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private Vector3 hookLocation;
    private bool hookPulling = false;
    private bool hookReady = true;
    private bool isGrounded = false;

    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    // Movement speed in units/sec.
    public float pullInSpeed = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;


    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();

        // lock cursor to window
        // TODO: on pause, unlock cursor.
        Cursor.lockState = CursorLockMode.Locked;

        roundedTime = (int)timeRemaining;
        timeForUI = roundedTime;
    }

    // Update is called once per frame
    void Update()
    {
        // update timer
        timeRemaining -= Time.deltaTime;
        roundedTime = Mathf.FloorToInt(timeRemaining);
        if (prevRoundedTime != roundedTime)
        {
            timeForUI = roundedTime;
        }
        prevRoundedTime = roundedTime;

        TurnCamera();
        if (isGrounded)
        {
            Walk();
        }

        // on pressing escape
        if (Input.GetAxis("Pause") > 0)
        {
            PauseGame();
        }

        // on click, attempt to shoot grappling hook
        if (Input.GetAxis("LaunchHook") > 0)
        {
            if (hookReady)
            {
                ShootHook();
            }
        }

        // begin reeling player in to hook location
        if (hookPulling)
        {
            PullToHook();
        }
    }

    private void Walk()
    {
        body.velocity = (transform.forward * Input.GetAxis("Vertical")) * moveSpeed * Time.fixedDeltaTime;
        body.velocity += (transform.right * Input.GetAxis("Horizontal")) * moveSpeed * Time.fixedDeltaTime;

        //body.velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * moveSpeed * Time.fixedDeltaTime;
    }

    void OnGUI()
    {
        // draw crosshair
        float xMin = (Screen.width / 2) - (crosshairImage.width / 32);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 32);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width / 16, crosshairImage.height / 16), crosshairImage);

        GUI.Label(new Rect(0, 0, 100, 100), timeForUI.ToString());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hookPulling)
        {
            StopPulling();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    public void OnFeetCollisionEnter()
    {
        Debug.Log("feet on ground");
        isGrounded = true;
    }

    public void OnFeetCollisionExit()
    {
        Debug.Log("feet off ground");
        isGrounded = false;
    }

    private void PauseGame()
    {
        Debug.Log("Pausing...");
    }

    private void PullToHook()
    {
        // Distance moved = time * speed.
        float distCovered = (Time.time - startTime) * pullInSpeed;

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
            hookReady = false;
            GetComponent<Rigidbody>().useGravity = false;

            startMarker = transform;

            // Keep a note of the time the movement started.
            startTime = Time.time;

            // Calculate the journey length.
            journeyLength = Vector3.Distance(startMarker.position, hookLocation);
        }
    }

    private void StopPulling()
    {
        print("stopping pulling");
        hookPulling = false;
        hookReady = true;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void TurnCamera()
    {
        yaw += mouseSpeedH * Input.GetAxis("Mouse X");
        pitch -= mouseSpeedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
