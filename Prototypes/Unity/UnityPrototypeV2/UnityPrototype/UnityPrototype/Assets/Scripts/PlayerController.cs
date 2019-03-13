using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 
// fix bug when grappling to floor - player sort of vibrates
// pause UI
// intro menu UI
// victory UI
// failure UI
// hook projectile
// add next levels

public class PlayerController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public GameObject hook;
    public Texture2D crosshairImage;
    public Canvas deathUI;
    private Rigidbody body;
    

    private enum State
    {
        HookMoving,
        HookPulling,
        HookReady,
        Paused
    }

    State state;

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

    // Transforms to act as start and end markers for the journey.
    public Transform startMarker;
    public Transform endMarker;

    // Movement speed in units/sec.
    public float pullInSpeed = 1.0F;
    public float hookLength = 50f;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;


    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        hook.SetActive(false);
        deathUI.enabled = false;

        state = State.HookReady;

        // lock cursor to window
        // TODO: on pause, unlock cursor.
        Cursor.lockState = CursorLockMode.Locked;

        roundedTime = (int)timeRemaining;
        timeForUI = roundedTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Paused)
        {

        }

        switch (state)
        {
            case State.HookMoving:
                
                break;
            case State.HookPulling:
                PullToHook();
                break;
            case State.HookReady:
                if (Input.GetAxis("LaunchHook") > 0)
                {
                    ShootHook();
                }
                break;
            default:
                break;
        }


        // update timer
        timeRemaining -= Time.deltaTime;
        roundedTime = Mathf.FloorToInt(timeRemaining);
        if (prevRoundedTime != roundedTime)
        {
            timeForUI = roundedTime;
        }
        prevRoundedTime = roundedTime;

        if (timeRemaining <= 0)
        {
            Debug.Log("time up");
            Fail();
        }

        if (state != State.Paused)
        {
            TurnCamera();
        }

        // on pressing escape
        if (Input.GetAxis("Pause") > 0)
        {
            PauseGame();
        }
    }

    void OnGUI()
    {
        // draw crosshair
        float xMin = (Screen.width / 2) - (crosshairImage.width / 32);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 32);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width / 16, crosshairImage.height / 16), crosshairImage);

        GUI.Label(new Rect(20, 20, 100, 100), timeForUI.ToString());

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Killbox"))
        {
            Fail();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision detected");
        if (collision.collider.CompareTag("Killbox"))
        {
            Fail();
        }
        if (state == State.HookPulling)
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
    }

    public void OnFeetCollisionExit()
    {
        Debug.Log("feet off ground");
    }

    private void Fail()
    {
        state = State.Paused;
        Time.timeScale = 0;
        deathUI.enabled = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void PauseGame()
    {
        Debug.Log("Pausing...");
        Cursor.lockState = CursorLockMode.None;
    }

    private void PullToHook()
    {
        
        // Draw a line to represent the grapple rope
        // TODO: change this to something non-debug
        Debug.DrawRay(firstPersonCamera.transform.position, firstPersonCamera.transform.position - hookLocation, Color.black);

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

        if (Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.TransformDirection(Vector3.forward), out hit, hookLength))
        {
            hookLocation = hit.point;
            state = State.HookPulling;
            body.useGravity = false;

            startMarker = transform;

            // Keep a note of the time the movement started.
            startTime = Time.time;

            // Calculate the journey length.
            journeyLength = Vector3.Distance(startMarker.position, hookLocation);
        }
        
        /*
        hook.SetActive(true);
        hook.transform.SetPositionAndRotation(firstPersonCamera.transform.position, firstPersonCamera.transform.rotation);
        hook.GetComponent<Rigidbody>().velocity = hook.transform.forward * 10;
        */
    }

    private void StopPulling()
    {
        print("stopping pulling");
        state = State.HookReady;
        body.useGravity = true;
    }

    private void TurnCamera()
    {
        yaw += mouseSpeedH * Input.GetAxis("Mouse X");
        pitch -= mouseSpeedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
