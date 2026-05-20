using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    private GameObject gMref;
    private List<GameObject> myList;
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
        gravity = true; //default all planets into gravity calculations
        if (gravity)    //only add to stellar gravity if enabled
        {
            gMref = GameObject.Find("GameManager");
            myGMScript = gMref.GetComponent<GameManagerScript>();
            myList = myGMScript.list_gos;
            myList.Add(gameObject); //TODO: need to delete once object removed?}
            rb = gameObject.GetComponent<Rigidbody>();
            mass = rb.mass;
        }
    }

    void FixedUpdate()
    {
        if (rotation){  //Planet rotation
            transform.Rotate(0, rotation_spd * Time.deltaTime, 0);
        }
    }
}
