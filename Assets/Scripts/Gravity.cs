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
        toDraw *= -1;
        visualGravityVector.SetPosition(1, toDraw);
    }

    public void GravityVectorSum(Vector3 newForce)
    {
        gravityVectorSum += newForce;
    }
    public void ApplyGravity()
    {
        GetComponent<Rigidbody>().AddForce(gravityVectorSum);
        DrawForceVector(gravityVectorSum);
        gravityVectorSum = Vector3.zero;    //reset for next cycle
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gravityVectorSum = Vector3.zero;

        visualGravityVector = gameObject.GetOrAddComponent<LineRenderer>();
        visualGravityVector.useWorldSpace = false;
        visualGravityVector.SetPosition(0, transform.position); //start to object

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
    {
        visualGravityVector.SetPosition(0, transform.position); //update start pos
    }
}
