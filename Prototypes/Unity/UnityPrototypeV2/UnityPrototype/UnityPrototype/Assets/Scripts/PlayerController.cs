using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 
// fix bug when grappling to floor - player sort of vibrates
// victory UI
// hook projectile
// 

public class PlayerController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public GameObject hook;
    public Texture2D crosshairImage;
    public Canvas deathUI;
    public Canvas pauseUI;
    private Rigidbody body;
    
    // defines a state machine to ensure player doesn't hook when he shouldn't
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

    // Transforms to act as start and end markers for the journey.
    private Vector3 startMarker;
    private Vector3 hookLocation;

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

        // hide death and pause screens
        deathUI.enabled = false;
        pauseUI.enabled = false;

        state = State.HookReady;

        // unpause time
        Time.timeScale = 1;

        // lock cursor to window
        Cursor.lockState = CursorLockMode.Locked;

        // set up timer variables
        roundedTime = (int)timeRemaining;
        timeForUI = roundedTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // check state to decide what to do this update frame
        switch (state)
        {
            case State.HookMoving:
                
                break;
            // if HookPulling then lerp the player one frame closer to destination
            case State.HookPulling:
                if (Input.GetAxis("BreakHook") > 0)
                {
                    StopPulling();
                }
                PullToHook();
                break;
            // if HookReady then check for user input to launch hook
            case State.HookReady:
                if (Input.GetAxis("LaunchHook") > 0)
                {
                    ShootHook();
                }
                break;
            default:
                break;
        }        

        if (state != State.Paused)
        {
            // update timer
            timeRemaining -= Time.deltaTime;
            roundedTime = Mathf.FloorToInt(timeRemaining);
            if (prevRoundedTime != roundedTime)
            {
                timeForUI = roundedTime;
            }
            prevRoundedTime = roundedTime;

            // if time <= 0, bring up fail screen
            if (timeRemaining <= 0)
            {
                Fail();
            }

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

        if (state != State.Paused)
        {
            // draw crosshair
            float xMin = (Screen.width / 2) - (crosshairImage.width / 32);
            float yMin = (Screen.height / 2) - (crosshairImage.height / 32);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width / 16, crosshairImage.height / 16), crosshairImage);
        }

        // draw time remaining to screen
        // TODO: add this as a permanent UI element - saves cpu time and easier customisation
        GUI.Label(new Rect(20, 20, 100, 100), timeForUI.ToString());
    }

    private void OnTriggerEnter(Collider other)
    {
        // if player touches killbox
        if (other.CompareTag("Killbox"))
        {
            Fail();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Killbox"))
        {
            Fail();
        }
        // if player is reeling in to hook and touches a collider, stop pulling
        if (state == State.HookPulling)
        {
            StopPulling();
        }
        // if player touches floor, freeze position - position is unfrozen when hooking
        if (collision.GetContact(0).normal.y >= 0.7f)
        {
            body.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ
                | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    // die: set state to paused, pause time, bring up death UI, unlock cursor
    private void Fail()
    {
        state = State.Paused;
        Time.timeScale = 0;
        deathUI.enabled = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // pause: set state to pause, pause time, bring up pause UI, unlock cursor
    private void PauseGame()
    {
        state = State.Paused;
        Time.timeScale = 0;
        Debug.Log("Pausing...");
        pauseUI.enabled = true;
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
        //body.AddForce((hookLocation - startMarker).normalized * 60);

        transform.position = Vector3.Lerp(startMarker, hookLocation, fracJourney);
        
        
    }

    private void ShootHook()
    {
        
        RaycastHit hit;

        if (Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.TransformDirection(Vector3.forward), out hit, hookLength))
        {
            hookLocation = hit.point;
            state = State.HookPulling;
            body.useGravity = false;

            startMarker = transform.position;

            // Keep a note of the time the movement started.
            startTime = Time.time;

            // Calculate the journey length.
            journeyLength = Vector3.Distance(startMarker, hookLocation);

            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
