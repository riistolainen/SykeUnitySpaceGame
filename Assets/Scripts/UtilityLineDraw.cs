using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

//TODO: Only one linerenderer per object -> Use the same linerenderer for all objects lines ()
//OR
//Create a separate object for linerendering that spawns new linerenderers
//TODO: do not use lineRenderer
//https://docs.unity3d.com/ScriptReference/GL.html
//https://gamedev.stackexchange.com/questions/96964/how-to-correctly-draw-a-line-in-unity


public class UtilityLineDraw : MonoBehaviour
{
    
    //TODO: get/set enable/disable to eventhandling?
    public bool enableAccelerationVector = true;
    public LineRenderer visualAccelerationVector;
    
    public bool enableGravityVector = false;
    public LineRenderer visualGravityVector;

    private Gradient gradient = new Gradient();
    private GradientColorKey[] lowGradient = new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.yellow, 1.0f) };
    private GradientColorKey[] medGradient = new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(Color.orange, 1.0f) };
    private GradientColorKey[] highGradient = new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(Color.orange, 0.75f), new GradientColorKey(Color.red, 1.0f) };
    private GradientAlphaKey[] alphaGradient = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) };
    
    public enum gravityScale {
        low = 10,
        med = 100,
        high = 1000
    };

    //TODO: Scale line length as square of distance above the thresholds to stop the lines from growing too large. Instead have the color notate the strength (gradients are good for this about)
    public Gradient SetColorGradient(float scale)
    {
        if (scale < (float)gravityScale.low)
        {
            gradient.SetKeys(
                lowGradient,
                alphaGradient
            );
        }
        else if (scale > (float)gravityScale.low && scale < (float)gravityScale.med)
        {
            gradient.SetKeys(
                medGradient,
                alphaGradient
            );
        }
        else if (scale > (float)gravityScale.med)
        {
            gradient.SetKeys(
                highGradient,
                alphaGradient
            );
        }
        else { Debug.LogWarning("WTF-colorgradients"); }
        return gradient;
    }

    public void DrawForceVector(Vector3 toDraw) //TODO: Common heritage to all gravitybodies?
    {
        if (enableGravityVector)
        {
            visualGravityVector.colorGradient = SetColorGradient(toDraw.magnitude);
            visualGravityVector.SetPosition(0, Vector3.zero); //update start position to object
            visualGravityVector.SetPosition(1, toDraw.normalized * Mathf.Clamp(toDraw.magnitude, 0, 25));   //TODO: trying to limit the size of drawn gravitylines, but unable to on the planet for some reason
        }
        else if (enableAccelerationVector)  //TODO: If multiple lines, this needs to be separate if, NOT else if
        {
            gameObject.TryGetComponent<Rigidbody>(out Rigidbody myRB);
            if (myRB != null)
            {
                float acc = toDraw.magnitude / myRB.mass;   //acceleration scale of force being applied

                visualAccelerationVector.colorGradient = SetColorGradient(toDraw.magnitude);    //TODO: different gradients for acceleration vector?
                visualAccelerationVector.SetPosition(0, Vector3.zero); //update start position to object   //TODO: start vector from surace of model, not inside:: + (Vector3.Scale(toDraw.normalized, transform.localScale/2))
                visualAccelerationVector.SetPosition(1, toDraw.normalized * Mathf.Clamp(acc, 0, 25));
            }
            else { Debug.LogError("UtilityLineDraw.cs: Rigidbody not found!"); }
        }
    }

    void Start()
    {
        //TODO: Currently the IF does nothing as only one linerenderer is present - if separate lines will be drawn needs functionality
        if (enableGravityVector)
        {
            visualGravityVector = gameObject.GetOrAddComponent<LineRenderer>();
            visualGravityVector.useWorldSpace = false;
            visualGravityVector.SetPosition(0, transform.localPosition); //start to object
            visualGravityVector.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
        }
        else if (enableAccelerationVector)
        {
            visualAccelerationVector = gameObject.GetOrAddComponent<LineRenderer>();
            visualAccelerationVector.useWorldSpace = false;
            visualAccelerationVector.SetPosition(0, transform.localPosition); //start to object
            visualAccelerationVector.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
        }
    }
}
