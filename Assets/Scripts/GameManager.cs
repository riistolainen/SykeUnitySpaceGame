//TODO: use tryGetComponent over GetComponent and manual null check in _all_ scripts?

using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject Planet;
    public GameObject Player;
    public List<GameObject> list_gos; //list of gravity objects that are used for stellar physics calc

    //effectiveGravity = G/(1+(distance/degradingFactor))
    public float G;
    public float degradingFactor;
    //public float[,] gravityForces;

    //public List<LineRenderer> linesList;
    //public LineRenderer addingLine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //effectiveGravity = G/(1+(distance/degradingFactor))
        G = 6.674f * 10E-11f; // * 10e22f; //scaling factor of 10e22 to get usable celestial mass numbers;
        degradingFactor = 1000;

        //addingLine = gameObject.GetOrAddComponent<LineRenderer>();

        // TODO: instantiate level in sceneManager instead of manually placing objects?
        // newGO = Instantiate(planet2, new Vector3(0, 0, 0), Quaternion.identity);

        //list_gos.AddRange(GameObject.FindGameObjectsWithTag("GravityBody")); //prepopulate with scene
        Debug.Log("Initial gravity objects: " + list_gos.Count);
    }

    public bool AddMe(GameObject goesToList)    //track gravity objects
    {
        if (!list_gos.Contains(goesToList))
        {
            list_gos.Add(goesToList);
            Debug.Log("ListedGO: " + goesToList.name);
            return true;
        }
        return false;
    }


    /*UNUSED
    public void DrawLine(Vector3 from, Vector3 to)
    { //TODO: add return values for debugging?
        addingLine.useWorldSpace = false;
        addingLine.SetPosition(0, from);
        addingLine.SetPosition(1, to);
        linesList.Add(addingLine);
    }
    */

    private void FixedUpdate()
    {

        /*  START:  ### GRAVITY ### */
        if (list_gos.Count > 1) //only when two or more gravity objects
        {   //Apply gravity from each to each
            for (int index = 0; index < list_gos.Count; index++)    //TODO: refactor the calculations - not correct currently on summing up the forces
            {
                //reset force calculation for index-GO
                //Vector3 forceVector;

                GameObject toGO = list_gos[index];

                for (int jindex = 0; jindex < list_gos.Count; jindex++)
                {
                    if (index != jindex) //not self - TODO: does not work?
                    {
                        GameObject fromGO = list_gos[jindex];

                        float dist = Vector3.Distance(toGO.transform.position, fromGO.transform.position);

                        Vector3 dir = toGO.transform.position - fromGO.transform.position; //(to, from)
                        float effG = Time.fixedDeltaTime*(G+toGO.GetComponent<Rigidbody>().mass*fromGO.GetComponent<Rigidbody>().mass)/(1+(dist/degradingFactor)); //(500 away gravity is 1/2 G) //Too small?// Time.fixedDeltaTime * G * ((oneRB.mass * otherRB.mass) / (1f + (dist * dist))); //Time.fixedDeltaTime default is 0.02, so limit force application by time interval
                        fromGO.GetComponent<GravityScript>().GravityVectorSum(Vector3.Scale(dir.normalized, new Vector3(effG, effG, effG)));    //sum forcevectors before applying

                        Debug.Log("#Gravity#    " + toGO.name + " -> " + fromGO.name + " Direction:  " + dir + ", Distance:    " + dist + ", effG:   " + effG);
                        Debug.Log("NewForce: " +Vector3.Scale(dir.normalized, new Vector3(effG, effG, effG)) +" NormDir: " + dir.normalized + " AmountMult: " + new Vector3(effG, effG, effG));
                    }
                }
                //one index done over jindex
            }
            //TODO: Clean list_gos? ie. each object to remove itself from list on OnDestroy()?
        }
        /*  EOF:    ### GRAVITY ### */


        /*  START:  ### LINES FADE   ### */
        /*
        for (int i = 0; linesList.Count < i; i++)
        {
            if (linesList[i].startWidth > 0.2)
            {
                linesList[i].startWidth= linesList[i].startWidth - 0.1f;
                linesList[i].endWidth = linesList[i].startWidth - 0.1f;
            }
            else
            {
                linesList.RemoveAt(i);
            }
        }*/
        /*  EOF:  ### LINES FADE   ### */

    }
}
