using System.Collections.Generic;
using UnityEngine;

public class SpaceShipScript : MonoBehaviour
{
    public bool gravity = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gravity)
        {
            gameObject.AddComponent<GravityScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
    
    }
}
