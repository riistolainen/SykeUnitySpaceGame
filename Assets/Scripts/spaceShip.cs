using UnityEngine;
using UnityEngine.InputSystem;

//TODO1: controls to player so they don't have to be on each separate ss?
//TODO: PRIO Gyro UI that shows current acceleration of SS-object related to "reset" point

public class SpaceShipScript : MonoBehaviour
{
    public bool gravity = true;

    //START:: SCENE INIT
    public Vector3 currentSpeed = Vector3.zero;
    public Vector3 currentAngularVelocity = Vector3.zero;
    public Vector3 initSpeed = Vector3.zero;
    //END:: SCENE INIT

    //START:: CONTROL
    public bool thrustActive = false;
    public bool rollActive = false;
    public bool yawActive = false;
    public bool pitchActive = false;
    //END:: CONTROL

    //TODO: Currently there is a clamp on input settings for all relevant axis. 1) change this based on the spaceship OR 2) remove clamp from inputs and add it here?
    // Currently moving mouse faster may allow moving ship faster... so mouse movement must be clamped. Using analog control requires more work than digital direction control.
    // Do the ships thrusters work with part power or are they ON/OFF with specific burn times?
    // Allows ship design variation through limitations. Spool-up-time for thrusters - minimum thrust acquired etc. before getting to analog control (always available precise vectoring)

    //START:: SPACESHIP STATS
    public float thrustMain = 10; //power of thruster
    public float thrustUtility = 1;
    public float thrustRoll = 1;
    public float thrustYaw = 1;
    public float thrustPitch = 1;

    //END:: SPACESHIP STATS

    private Rigidbody myRB;

    public void Pilot(float thrust, float roll, float yaw, float pitch)
    {
        //TODO: Adjust mass for piloting only (multithread issues?)

        //TODO: Cap forces to ship propulsion values
        if (thrust != 0f)
        {
            myRB.AddForce(transform.forward * thrust * thrustMain);   //"forward" z-axis... I think. Thruster power in kN... scaling has been done on stellar masses
            thrustActive = false;
            Debug.Log("#Thrust#    " + gameObject.name + "/" + "   -> " + " Direction:  " + transform.forward + ", Power: " + thrustMain);
        }

        if (roll != 0f)
        {
            myRB.AddRelativeTorque(Vector3.forward * roll * thrustRoll, ForceMode.Impulse);
            rollActive = false;
            Debug.Log("Roll: " + roll);
        }

        if (yaw != 0f)
        {
            myRB.AddRelativeTorque(Vector3.up * yaw * thrustYaw, ForceMode.Impulse);
            yawActive = false;
            Debug.Log("Yaw: " + yaw);
        }

        if (pitch != 0f)
        {
            myRB.AddRelativeTorque(Vector3.right * pitch * thrustPitch, ForceMode.Impulse);
            pitchActive = false;
            Debug.Log("Pitch: " + pitch);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.TryGetComponent<Rigidbody>(out myRB);
        if (myRB == null) { Debug.LogError("Spaceship: No Rigidbody found!"); }
        else
        {
            myRB.angularDamping = 0f;   //Default 0.5 - space has no air
            myRB.maxAngularVelocity =   100000000f; //up max speeds
            myRB.maxLinearVelocity =    100000000f;
        }

        if (gravity)
        {
            gameObject.AddComponent<GravityScript>();
        }

        if (initSpeed != Vector3.zero)  // initial orbit - defined at the start of each level
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = initSpeed;
        }


        //Random color to spaceship block?
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_Color", Random.ColorHSV());
        gameObject.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }

    private void Update()   //TODO: deltatime multiplier to thrust. Count time thrust is pressed from enable to disable on button raised after it isPressed
    {

    }

    private void FixedUpdate()
    {
        currentAngularVelocity = myRB.angularVelocity;
        currentSpeed = myRB.linearVelocity;
        
    }
}