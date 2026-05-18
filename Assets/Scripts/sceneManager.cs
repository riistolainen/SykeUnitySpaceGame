using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;

public class sceneManager : MonoBehaviour
{
    public List<GameObject> list_gos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: instantiate level in sceneManager instead of manually placing objects?
        // newGO = Instantiate(planet2, new Vector3(0, 0, 0), Quaternion.identity);

        list_gos = new List<GameObject> (GameObject.FindGameObjectsWithTag("GravityBody")); //public list of gravity objects, new objects are added when created during gameplay -- more performative than always searching all objects
    }

    // Update is called once per frame
    void Update()
    {

    }
}
