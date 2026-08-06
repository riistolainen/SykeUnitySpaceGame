using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class GravityScript : MonoBehaviour
{
    public bool debug = false;  //local script debug: enable/disable
    public bool gravity = true;
    public float mass;
    
    public LineRenderer visualGravityVector;
    
    Gradient gradient = new Gradient();
    GradientColorKey[] lowGradient = new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.yellow, 1.0f) };
    GradientColorKey[] medGradient = new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.orange, 1.0f) };
    GradientColorKey[] highGradient = new GradientColorKey[] { new GradientColorKey(Color.orange, 0.0f), new GradientColorKey(Color.red, 1.0f) };
    GradientAlphaKey[] alphaGradient = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
    public float low = 10, med = 50, high = 100;
    public Vector3 gravityVectorSum;

    private GameObject gMref;
    private GameManagerScript myGMScript;
    private Rigidbody rb;

    public void SetColorGradient(float scale)
    {
        if (scale < low) {
            gradient.SetKeys(
                lowGradient,
                alphaGradient
            );
        }
        else if (scale > low && scale < med) {
            gradient.SetKeys(
                medGradient,
                alphaGradient
            );
        }
        else if (scale > med) {
            gradient.SetKeys(
                highGradient,
                alphaGradient
            );
        }
        else { Debug.Log("WTF-colorgradients"); }
        visualGravityVector.colorGradient = gradient;
    }

    public void DrawForceVector(Vector3 toDraw) //TODO: Cmmon heritage to all gravitybodies?
    {
        SetColorGradient(toDraw.magnitude);
        visualGravityVector.SetPosition(1, Vector3.zero); //update start position to object
        visualGravityVector.SetPosition(0, toDraw);
    }

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
            GetComponent<Rigidbody>().AddForce(gravityVectorSum, ForceMode.Impulse);   //add gravity force
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

        visualGravityVector = gameObject.GetOrAddComponent<LineRenderer>();
        visualGravityVector.useWorldSpace = false;
        visualGravityVector.SetPosition(0, transform.localPosition); //start to object
        visualGravityVector.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));

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
        DrawForceVector(gravityVectorSum);  //draw visualization
        ApplyGravity(); //each GO apply their own calculated gravityforce themselves; gamemanager calculates the forces
    }
}
