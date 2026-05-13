using Unity.Entities;
using UnityEngine;

public class sceneManager : MonoBehaviour
{
    public GameObject planet2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planet2 = Instantiate(planet2, new Vector3(0, 0, 0), Quaternion.identity); //wtf

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
