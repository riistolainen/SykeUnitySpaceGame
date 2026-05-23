using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GameManagerScript: MonoBehaviour
{
    public GameObject Planet;
    public GameObject Player;
    public List<GameObject> list_gos; //list of gravity objects that are used for stellar physics calc

    public float G = 6.674f * 10E-11f; // * 10e22f; //scaling factor of 10e22 to get usable celestial mass numbers;

    public List<LineRenderer> linesList;
    public LineRenderer addingLine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        addingLine = gameObject.GetOrAddComponent<LineRenderer>();

        // TODO: instantiate level in sceneManager instead of manually placing objects?
        // newGO = Instantiate(planet2, new Vector3(0, 0, 0), Quaternion.identity);

        //list_gos.AddRange(GameObject.FindGameObjectsWithTag("GravityBody")); //prepopulate with scene
        Debug.Log("Initial gravity objects: " +list_gos.Count);
    }

    public bool AddMe(GameObject goesToList)    //track gravity objects
    {
        if (!list_gos.Contains(goesToList))
        {
            list_gos.Add(goesToList);
            Debug.Log("ListedGO: " +goesToList.name);
            return true;
        }
        return false;
    }

    public void DrawLine(Vector3 from, Vector3 to) 
    { //TODO: add return values for debugging?
        addingLine.useWorldSpace = true;
        addingLine.SetPosition(0, from);
        addingLine.SetPosition(1, to);
        linesList.Add(addingLine);
    }

    private void FixedUpdate()
    {
        /*  START:  ### GRAVITY ### */
        if (list_gos.Count > 1) //only when two or more gravity objects
        {   //Apply gravity from each to each
            for (int index = 0; index < list_gos.Count; index++)    //TODO: refactor the calculations - not correct currently on summing up the forces
            {
                Vector3 forceVector;   //reset force calculation for each gravity object
                forceVector = new Vector3(0f, 0f, 0f);

                Rigidbody otherRB = list_gos[index].GetComponent<Rigidbody>();
                
                for (int jindex = 0; jindex < list_gos.Count; jindex++)
                {
                    if (index != jindex) //not self - TODO: does not work
                    {
                        Rigidbody oneRB = list_gos[jindex].GetComponent<Rigidbody>();
                        float dist = Vector3.Distance(list_gos[index].transform.position, list_gos[jindex].transform.position);
                        
                        Vector3 dir = list_gos[index].transform.position - list_gos[jindex].transform.position;
                        float effG = Time.fixedDeltaTime * G * ((oneRB.mass * otherRB.mass) / (1f + (dist * dist))); //Time.fixedDeltaTime default is 0.02, so limit force application by time interval
                        forceVector += Vector3.Scale(dir, new Vector3(effG, effG, effG));

                        Debug.Log("#Gravity#    " + list_gos[index].name + " -> " + list_gos[jindex].name + " Direction:  " + dir + ", Distance:    " + dist + ", effG:   " + effG);
                    }
                }
                //one index done over jindex
                otherRB.AddForce(forceVector);  //add sum of all gravity force to GO
                list_gos[index].GetComponent<GravityScript>().DrawForceVector(forceVector);    //ask GO to draw(/update) the applied force vector visually

            }
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
