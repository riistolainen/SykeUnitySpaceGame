using System;
using Unity.VisualScripting;
using UnityEngine;

public class GravityScript : MonoBehaviour
{
    public bool gravity = true;
    public float mass;
    public LineRenderer visualGravityVector;
    public Vector3 gravityVectorSum;

    private GameObject gMref;
    private GameManagerScript myGMScript;
    private Rigidbody rb;

    public void DrawForceVector(Vector3 toDraw) //TODO: Cmmon heritage to all gravitybodies?
    {
        toDraw *= -1;   //TODO: seems to draw vector always to same direction regardless of trying to reverse it
        visualGravityVector.SetPosition(0, transform.position); //update start position to object
        visualGravityVector.SetPosition(1, toDraw);
    }

    public void GravityVectorSum(Vector3 newForce)
    {
        gravityVectorSum += newForce;
    }

    public void ApplyGravity()
    {
        if (gravityVectorSum != Vector3.zero)
        {
            Debug.Log(gravityVectorSum.ToString());
        }
        GetComponent<Rigidbody>().AddForce(gravityVectorSum);   //add gravity force
        gravityVectorSum = Vector3.zero;    //reset force for next cycle
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gravityVectorSum = Vector3.zero;

        visualGravityVector = gameObject.GetOrAddComponent<LineRenderer>();
        visualGravityVector.useWorldSpace = false;
        visualGravityVector.SetPosition(0, transform.localPosition); //start to object

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
                        Debug.LogError("ERROR: Could not Add GravityObject: " + gameObject.name);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {   //TODO: option to enable/disable drawing of vectors
        DrawForceVector(gravityVectorSum);  //draw visualization
        ApplyGravity(); //each GO apply their own calculated gravityforce themselves; gamemanager calculates the forces
    }
}
