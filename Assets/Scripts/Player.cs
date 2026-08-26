using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public enum stateControl
    {
        UI = 1,
        Pilot = 2,
        FreeCam = 3
    };

    public stateControl currentState = stateControl.UI;

    /*TODO: NOT NEEDED?
    //GameManager stuff
    public GameObject gMref;
    private GameManagerScript myGMScript;

    //Spaceship stuff
    private GameObject spaceship_obj;
    private SpaceShipScript spaceShipScript;
    private Rigidbody spaceship_RB;
    */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /* TODO: NOT NEEDED?
        gMref = GameObject.Find("GameManager");
        myGMScript = gMref.GetComponent<GameManagerScript>();

        spaceship_obj = gameObject.transform.Find("SpaceShip").gameObject;  //child-parent relation through gameObjects Transform component in Unity
        if (spaceship_obj == null)
        {
            Debug.LogWarning("PlayerStart() FAILED: spaceship_obj NULL");
        }
        else
        {
            spaceship_RB = spaceship_obj.GetComponent<Rigidbody>();
            spaceShipScript = spaceship_obj.GetComponent<SpaceShipScript>();
        }
        */
    }
}