using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class Cloud : MonoBehaviour
{
    public VisualEffect cloud;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cloud.Play();

        cloud.Stop();
    }
}
