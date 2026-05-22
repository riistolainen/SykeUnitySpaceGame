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

        //list_gos.AddRange(GameObject.FindGameObjectsWithTag("GravityBody")); //prepopulate with scene

        Debug.Log("Initial gravity objects: " +list_gos.Count);
    }

    public bool AddMe(GameObject goesToList)
    {
        if (!list_gos.Contains(goesToList))
        {
            list_gos.Add(goesToList);
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        /*  START:  ### GRAVITY ### */
        if (list_gos.Count > 1) //only when two or more gravity objects
        {   //Apply gravity from each to each
            for (int index = 0; index < list_gos.Count; index++)
            {
                Rigidbody otherRB = list_gos[index].GetComponent<Rigidbody>();
                
                for (int jindex = 0; jindex < list_gos.Count; jindex++)
                {
                    if (index != jindex) //not self - TODO: does not work
                    {
                        Rigidbody oneRB = list_gos[jindex].GetComponent<Rigidbody>();
                        float dist = Vector3.Distance(list_gos[index].transform.position, list_gos[jindex].transform.position);
                        
                        Vector3 dir = list_gos[index].transform.position - list_gos[jindex].transform.position;
                        float effG = Time.fixedDeltaTime * G * ((oneRB.mass * otherRB.mass) / (1f + (dist * dist))); //Time.fixedDeltaTime default is 0.02, so limit force application by time interval
                        otherRB.AddForce(Vector3.Scale(dir, new Vector3(effG, effG, effG)));

                        Debug.Log("#Gravity#    " + list_gos[index].name + " -> " + list_gos[jindex].name + " Direction:  " + dir + ", Distance:    " + dist + ", effG:   " + effG);
                    }
                }
            }
        }
        /*  EOF:    ### GRAVITY ### */
    }
}
