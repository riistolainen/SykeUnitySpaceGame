using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    private GameObject gMref;
    private GameManagerScript myGMScript;
    private Rigidbody rb;

    public bool rotation = false;
    public float rotation_spd = 5f;
    public float rotation_angle = 2.3f;

    public bool gravity = true;
    public float mass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Planet rigidbody not found.");
        }
        else
        {
            mass = rb.mass;
        }

        if (gravity)    //only add to stellar gravity if enabled
        {
            gMref = GameObject.Find("GameManager");
            if (gMref == null)
            {
                Debug.LogWarning("GameManager object not found.");
            }
            else
            {
                myGMScript = gMref.GetComponent<GameManagerScript>();
                if (myGMScript == null)
                {
                    Debug.LogWarning("GameManagerScript not found.");
                }
                else
                {
                    if (!myGMScript.AddMe(gameObject))
                    {
                        Debug.LogError("ERROR: Could not Add GravityObject.");
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (rotation){  //Planet rotation
            transform.Rotate(0, rotation_spd * Time.deltaTime, 0);
        }
    }
}
