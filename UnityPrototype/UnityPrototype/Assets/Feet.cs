using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour {

    public PlayerController pc;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("feet touched enter");
        if (!collision.gameObject.CompareTag("Player"))
        {
            //pc.OnFeetCollisionEnter(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            //pc.OnFeetCollisionExit(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("feet touched enter");
        if (!other.CompareTag("Player"))
        {
            pc.OnFeetCollisionEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            pc.OnFeetCollisionExit();
        }
    }
}
