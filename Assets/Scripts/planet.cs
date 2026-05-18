using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Pool;

public class planet_Script : MonoBehaviour
{
    private GameObject sMref;
    private List<GameObject> listRef;

    private bool rotation;
    private float rotation_spd;
    private float rotation_angle;
    
    private bool gravity;
    public float mass;
    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sMref = GameObject.Find("SceneManager");
        listRef = sMref.GetComponent<List<GameObject>>();
        listRef.Add(gameObject); //TODO: need to delete once object removed?

        rotation = true;
        rotation_spd = 5f;
        rotation_angle = 2.3f;
        transform.Rotate(rotation_angle, 0, 0); //tilt celestial object
        
        gravity = true;
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (gravity)
        {
            //TODO: rotate through all gravity objects
            for (int index = 0; index < listRef.Count; index++)
            {
                Rigidbody otherRB = listRef[index].GetComponent<Rigidbody>();
                float dist = Vector3.Distance(gameObject.transform.position, listRef[index].transform.position);
                Vector3 direction = listRef[index].transform.position - this.gameObject.transform.position;
                float G = 6.674f * 10E-11f;
                float effG = G * ((mass * otherRB.mass) / dist * dist);
                rb.AddForce(effG * direction); //why not?
            }
        }

        //Planet rotation
        transform.Rotate(0, rotation_spd * Time.deltaTime, 0);        

    }
}
