using System;
using Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class SS_Script : MonoBehaviour
{
    bool thrustActive = false;
    InputAction thrust;
    float power = 1; //power of thruster
    GameObject spaceship_obj;
    GameObject player_obj;
    Rigidbody spaceship_RB;
    Rigidbody player_RB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thrust = InputSystem.actions.FindAction("MainThruster");
        player_obj = GameObject.Find("Player");
        spaceship_obj = GameObject.Find("Spaceship");
        spaceship_RB = GetComponent<Rigidbody>();
//        spaceShip_RB.transform.forward=player.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (thrust.IsPressed()) {
            thrustActive = true;
            /* v.2 
            spaceShip_RB.linearVelocity = spaceShip_RB.linearVelocity + transform.forward * power;
            print(spaceShip_RB.linearVelocity);
            */
            /* ###OLDER###
             Vector3 thrustVector3 = GameObject.FindGameObjectWithTag("Player").transform.position;
             thrustVector3=thrustVector3*power;
            */

        }
    }
    void FixedUpdate()
    {
        if (thrustActive) {
            spaceship_RB.AddForce(transform.forward * power);
            thrustActive = false;
        }

    }
}