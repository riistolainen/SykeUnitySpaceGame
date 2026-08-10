using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public bool gravity = true;
    public bool gravityVisual = false;
    public bool accelerationVisual = true;

    public bool rotation = true;
    public float rotation_spd = 5f;
    public float rotation_angle = 2.3f;

    public Vector3 initSpeed = new(0f,0f,0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gravity)
        {
            GravityScript handle = gameObject.AddComponent<GravityScript>();   //could not add in editor
            handle.enableAccelerationVector = accelerationVisual;
            handle.enableGravityVector = gravityVisual;
        }
        if(initSpeed!=Vector3.zero)
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = initSpeed;
        }
    }

    void FixedUpdate()
    {
        if (rotation)
        {  //Planet rotation
            transform.Rotate(0, rotation_spd * Time.deltaTime, 0);
        }
    }
}
