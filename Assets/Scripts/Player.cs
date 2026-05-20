using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public GameObject gMref;
    private List<GameObject> myList;
    public GameObject spaceship_obj;
    
    private bool thrustActive = false;
    private InputAction thrust;
    public float power = 100; //power of thruster

    private Rigidbody spaceship_RB;
    public float mass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //add to list of gravitybodies
        gMref = GameObject.Find("GameManager");
        myList = gMref.GetComponent<GameManagerScript>().list_gos;

        thrust = InputSystem.actions.FindAction("MainThruster");
        spaceship_obj = GameObject.Find("Spaceship");
        spaceship_RB = spaceship_obj.GetComponent<Rigidbody>();
        mass = spaceship_RB.mass;
        
        myList.Add(spaceship_obj); //TODO: need to delete once object removed?
    }

    // Update is called once per frame
    void Update()
    {
        if (thrust.IsPressed())
        {
            thrustActive = true;
        }
    }
    void FixedUpdate()
    {
        if (thrustActive)
        {
            spaceship_RB.AddForce(transform.forward * power);   //"forward" z-axis... I think. Thruster power in kN... scaling has been done on stellar masses
            thrustActive = false;
            Debug.Log("#Thrust#    " + gameObject.name + "/" + spaceship_obj.name + "   -> " + " Direction:  " + transform.forward + ", Power: " + power);
            //Debug.Log("#Gravity#    " + gameObject.name + " -> " + myList[index].name + " Direction:  " + direction + ", effG:   " + effG);
            /* v.2 
            spaceShip_RB.linearVelocity = spaceShip_RB.linearVelocity + transform.forward * power;
            print(spaceShip_RB.linearVelocity);
            */
            /* ###OLDER### ^newer upwards
             Vector3 thrustVector3 = GameObject.FindGameObjectWithTag("Player").transform.position;
             thrustVector3=thrustVector3*power;
            */
        }
    }
}