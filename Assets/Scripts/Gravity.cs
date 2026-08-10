using Unity.VisualScripting;
using UnityEngine;

//TODO: currently sums and then draws the sum force projected by gravity from object. When visualizing acceleration this breaks down for user as expectation is to see the acceleration experienced by the object, not excerted by the object

public class GravityScript : MonoBehaviour
{
    public bool debug = false;  //local script debug: enable/disable
    public bool gravity = true;
    public float mass;
    public Vector3 gravityVectorSum;

    private GameObject gMref;
    private GameManagerScript myGMScript;
    private Rigidbody rb;

    UtilityLineDraw myGravityVector;
    public bool enableAccelerationVector = true;
    public bool enableGravityVector = false;

    public void GravityVectorSum(Vector3 newForce)
    {
        if (newForce != Vector3.zero)
        {
            gravityVectorSum += newForce;
            if (debug) { Debug.Log(gameObject.name + " # New force: " + newForce.ToString() + " Sum: " + gravityVectorSum); }
        }
        else 
        {
            if (debug) { Debug.LogWarning(gameObject.name + " # GravityVectorSum: Zero newForce!"); }
        }
    }

    public void ApplyGravity()
    {
        if (gravityVectorSum != Vector3.zero)
        {
            if (debug) { Debug.Log(gravityVectorSum.ToString()); } //TODO: not working? expand
            if (TryGetComponent<Rigidbody>(out Rigidbody handle))
            {
                handle.AddForce(gravityVectorSum, ForceMode.Impulse);   //add gravity force
            }
            gravityVectorSum = Vector3.zero;    //reset force for next cycle
        }
        else
        {
            if (debug) { Debug.LogWarning(gameObject.name + " # ApplyGravity: Zero gravityVectorSUM!"); }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gravityVectorSum = Vector3.zero;
        myGravityVector = gameObject.GetOrAddComponent<UtilityLineDraw>();   //TODO: inheritance, pass constructor the bool values for enabling?
        myGravityVector.enableAccelerationVector = enableAccelerationVector;
        myGravityVector.enableGravityVector = enableGravityVector;

        rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            if (debug) { Debug.LogWarning("Planet rigidbody not found."); }
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
                if (debug) { Debug.LogWarning("GameManager object not found."); }
            }
            else
            {
                myGMScript = gMref.GetComponent<GameManagerScript>();
                if (myGMScript == null)
                {
                    if (debug) { Debug.LogWarning("GameManagerScript not found."); }
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
        myGravityVector.DrawForceVector(gravityVectorSum);  //draw visualization
        ApplyGravity(); //each GO apply their own calculated gravityforce themselves; gamemanager calculates the forces
    }
}
