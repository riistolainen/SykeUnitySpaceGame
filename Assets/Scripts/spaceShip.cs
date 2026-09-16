using UnityEngine;
using UnityEngine.InputSystem;

//TODO: controls to player so they don't have to be on each separate ss?
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


    //START:: SPACESHIP STATS
    public float mainThrustPower = 5; //power of thruster
    public float utilityThrustPower = 1;
    //END:: SPACESHIP STATS

    private Rigidbody myRB;

    public void Pilot(float thrust, float roll, float yaw, float pitch)
    {
        //Adjust mass for piloting only (multithread issues?)

        
        //TODO: Cap forces to ship propulsion values
        if (thrust != 0f)
        {
            myRB.AddForce(transform.forward * thrust * mainThrustPower);   //"forward" z-axis... I think. Thruster power in kN... scaling has been done on stellar masses
            thrustActive = false;
            Debug.Log("#Thrust#    " + gameObject.name + "/" + "   -> " + " Direction:  " + transform.forward + ", Power: " + mainThrustPower);
        }

        //TODO: Activate methods for roll, yaw, pitch - currently always ON
        //TODO: Fix axis for each

        myRB.mass = myRB.mass * 1000;

        if (pitch != 0f)
        {
            myRB.AddRelativeTorque(Vector3.right * pitch * utilityThrustPower, ForceMode.Impulse);
            pitchActive = false;
            Debug.Log("Pitch: " + pitch);
        }

        if (yaw != 0f)
        {
            myRB.AddRelativeTorque(Vector3.up * yaw * utilityThrustPower, ForceMode.Impulse);
            yawActive = false;
            Debug.Log("Yaw: " + yaw);
        }

        if (roll != 0f)
        {
            myRB.AddRelativeTorque(Vector3.forward * roll * utilityThrustPower, ForceMode.Impulse);
            rollActive = false;
            Debug.Log("Roll: " + roll);
        }

        myRB.mass = myRB.mass / 1000;
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