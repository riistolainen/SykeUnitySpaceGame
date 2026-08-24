using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceShipScript : MonoBehaviour
{
    public bool gravity = true;

    private bool thrustActive = false;
    private bool rollActive = false;
    private bool yawActive = false;
    private bool pitchActive = false;
    private InputAction thrust, roll, yaw, pitch;
    public float mainThrustPower = 5; //power of thruster
    public float utilityThrustPower = 1;
    public Vector3 initSpeed = new(0f,0f,0f);

    private Rigidbody myRB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.TryGetComponent<Rigidbody>(out myRB);
        if (myRB == null) { Debug.LogError("Spaceship: No Rigidbody found!"); }

        thrust = InputSystem.actions.FindAction("MainThruster");
        roll = InputSystem.actions.FindAction("Roll");
        yaw = InputSystem.actions.FindAction("Yaw");
        pitch = InputSystem.actions.FindAction("Pitch");

        if (gravity)
        {
            gameObject.AddComponent<GravityScript>();
        }

        if (initSpeed != Vector3.zero)  // initial orbit - defined at the start of each level
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = initSpeed;
        }
    }

    private void Update()   //TODO: deltatime multiplier to thrust. Count time thrust is pressed from enable to disable on button raised after it isPressed
    {
        if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)  //Only control ship when cursor is locked - when unlocked user is engaged with UI
        {//TODO: Fix camera when piloting; separate button to enable certain thrusters? CTRL/SHIFT/etc. or something else?
            if (thrust.IsPressed())
            {
                thrustActive = true;
            }

            if (roll.IsPressed())
            {
                rollActive = true;
            }

            if (yaw.IsPressed())
            {
                yawActive = true;
            }

            if (pitch.IsPressed())
            {
                pitchActive = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //TODO: Cap forces to ship propulsion values

        if (thrustActive)
        {
            myRB.AddForce(transform.forward * mainThrustPower);   //"forward" z-axis... I think. Thruster power in kN... scaling has been done on stellar masses
            thrustActive = false;
            Debug.Log("#Thrust#    " + gameObject.name + "/" + "   -> " + " Direction:  " + transform.forward + ", Power: " + mainThrustPower);
        }

        //TODO: Activate methods for roll, yaw, pitch - currently always ON
        //TODO: Fix axis for each

        if (pitchActive)
        {
            float inputValue = pitch.ReadValue<float>();
            myRB.AddRelativeTorque(Vector3.up * pitch.ReadValue<float>() * utilityThrustPower, ForceMode.Impulse);
            pitchActive = false;
            Debug.Log("Pitch: " + inputValue);
        }

        if (yawActive)
        {
            float inputValue = yaw.ReadValue<float>();
            myRB.AddRelativeTorque(Vector3.left * yaw.ReadValue<float>() * utilityThrustPower, ForceMode.Impulse);
            yawActive = false;
            Debug.Log("Yaw: " + inputValue);
        }

        if (rollActive)
        {
            float inputValue = roll.ReadValue<float>();
            myRB.AddRelativeTorque(Vector3.forward * roll.ReadValue<float>() * utilityThrustPower, ForceMode.Impulse);
            rollActive = false;
            Debug.Log("Roll: " + inputValue);
        }
    }
}