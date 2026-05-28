using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public bool rotation = false;
    public float rotation_spd = 5f;
    public float rotation_angle = 2.3f;

    public Vector3 initSpeed = new(0f,0f,0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.AddComponent<GravityScript>();   //could not add in editor
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
