using System;
using Unity.Physics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using System.Collections.Generic;

public class SS_Script : MonoBehaviour
{
    private GameObject sMref;

    private GameObject spaceship_obj;
    private GameObject player_obj;
    private Rigidbody player_RB;

    private bool thrustActive = false;
    private InputAction thrust;
    private float power = 1; //power of thruster

    private Rigidbody spaceship_RB;
    public float mass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sMref = GameObject.Find("sceneManager"); //add to list of gravitybodies
        sMref.list_gos.Add(gameObject); //TODO: need to delete once object removed?

        thrust = InputSystem.actions.FindAction("MainThruster");
        player_obj = GameObject.Find("Player");
        spaceship_obj = GameObject.Find("Spaceship");
        spaceship_RB = GetComponent<Rigidbody>();
        mass = spaceship_RB.mass;
    }

    // Update is called once per frame
    void Update()
    {
        if (thrust.IsPressed()) {
            thrustActive = true;
        }
    }
    void FixedUpdate()
    {
        if (thrustActive) {
            spaceship_RB.AddForce(transform.forward * power);
            thrustActive = false;

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