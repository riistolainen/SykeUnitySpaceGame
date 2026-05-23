using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//TODO: Move SpaceShip scripts to SpaceShip object?

public class PlayerScript : MonoBehaviour
{
    public GameObject gMref;
    private GameManagerScript myGMScript;
    
    private bool thrustActive = false;
    private InputAction thrust;
    public float power = 100; //power of thruster

    private GameObject spaceship_obj;
    private SpaceShipScript spaceShipScript;
    private Rigidbody spaceship_RB;
    public float mass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thrust = InputSystem.actions.FindAction("MainThruster");    //TODO: move to spaceship

        // ### Moved to separate gravity script that is added to gravity enabled objects:
        //add to list of gravitybodies
                gMref = GameObject.Find("GameManager");
                myGMScript = gMref.GetComponent<GameManagerScript>();

                spaceship_obj = gameObject.transform.Find("SpaceShip").gameObject;  //child-parent relation through gameObjects Transform component in Unity
                if (spaceship_obj == null)
                {
                    Debug.LogWarning("spaceship_obj NULL");
                }
                else
                {
                    spaceship_RB = spaceship_obj.GetComponent<Rigidbody>();
                    mass = spaceship_RB.mass;
                    spaceShipScript = spaceship_obj.GetComponent<SpaceShipScript>();

                    if (spaceShipScript.gravity)
                    {
                        if (!myGMScript.AddMe(spaceship_obj))
                        {
                            Debug.LogWarning("Could not add SpaceShip to Gravity list.");
                        }
                    }
                }
        
    }
    // Update is called once per frame
    void Update()
    {
        if (thrust.IsPressed()) //TODO: move to spaceship
        {
            thrustActive = true;
        }
    }
    void FixedUpdate()
    {
        if (thrustActive)   //TODO: Move to spaceship
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