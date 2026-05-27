using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceShipScript : MonoBehaviour
{
    public bool gravity = true;

    private bool thrustActive = false;
    private InputAction thrust;
    public float thrusterPower = 50; //power of thruster


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thrust = InputSystem.actions.FindAction("MainThruster");    //TODO: move to spaceship

        if (gravity)
        {
            gameObject.AddComponent<GravityScript>();
        }
    }

    private void Update()   //TODO: deltatime multiplier to thrust. Count time thrust is pressed from enable to disable on button raised after it isPressed
    {
        if (thrust.IsPressed()) //TODO: move to spaceship
        {
            thrustActive = true;
        }
    }

    private void FixedUpdate()
    {
        if (thrustActive)   //TODO: Move to spaceship
        {
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * thrusterPower);   //"forward" z-axis... I think. Thruster power in kN... scaling has been done on stellar masses
            thrustActive = false;
            Debug.Log("#Thrust#    " + gameObject.name + "/" + "   -> " + " Direction:  " + transform.forward + ", Power: " + thrusterPower);
        }
    }
}