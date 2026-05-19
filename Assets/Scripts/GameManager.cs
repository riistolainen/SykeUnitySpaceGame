using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript: MonoBehaviour
{
    public GameObject Planet;
    public GameObject Player;
    public List<GameObject> list_gos; //list of gravity objects that are used for stellar physics calc

    public float G = 6.674f * 10E-11f; // * 10e22f; //scaling factor of 10e22 to get usable celestial mass numbers;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: instantiate level in sceneManager instead of manually placing objects?
        // newGO = Instantiate(planet2, new Vector3(0, 0, 0), Quaternion.identity);

        list_gos = new List<GameObject>(GameObject.FindGameObjectsWithTag("GravityBody")); //public list of gravity objects, new objects are added when created during gameplay -- more performative than always searching all objects
        Debug.unityLogger.Log(list_gos);
    }
}
