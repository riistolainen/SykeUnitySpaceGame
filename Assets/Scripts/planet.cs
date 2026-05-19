using System.Collections.Generic;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    private GameObject gMref;
    private List<GameObject> myList;
    private Component compRef;
    private GameManagerScript myGMScript;
    private Rigidbody rb;

    public bool rotation;
    public float rotation_spd;
    public float rotation_angle;

    public bool gravity;
    public float mass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gMref = GameObject.Find("GameManager");
        myGMScript = gMref.GetComponent<GameManagerScript>();
        myList = myGMScript.list_gos;
        myList.Add(gameObject); //TODO: need to delete once object removed?

        rotation = true;
        rotation_spd = 5f;
        rotation_angle = 2.3f;
        transform.Rotate(rotation_angle, 0, 0); //tilt celestial object

        gravity = true;
        rb = gameObject.GetComponent<Rigidbody>();
        mass = rb.mass;
    }

    void FixedUpdate()
    {
        if (rotation){  //Planet rotation
            transform.Rotate(0, rotation_spd * Time.deltaTime, 0);
        }

        if (gravity && myList.Count > 0){   //TODO: rotate through all gravity objects and pull them in
            for (int index = 0; index < myList.Count; index++)
            {
                Rigidbody otherRB = myList[index].GetComponent<Rigidbody>();
                float dist = Vector3.Distance(gameObject.transform.position, myList[index].transform.position);
                if (dist > 0) { 
                    Vector3 direction = myList[index].transform.position - gameObject.transform.position;
                    float effG = myGMScript.G * ((mass * otherRB.mass) / (100f + (dist * dist)) );
                    rb.AddForce(Vector3.Scale(direction.normalized, new Vector3(effG, effG, effG))); //why not?
                }
            }
        }
    }
}
