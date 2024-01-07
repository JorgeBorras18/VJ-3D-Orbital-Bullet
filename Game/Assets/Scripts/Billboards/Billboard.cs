using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Look at camera always
public class Billboard : MonoBehaviour
{

    [SerializeField] private Transform Camera;

    private void Awake()
    {
        if (Camera == null) Camera = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Update()
    {
        transform.LookAt(transform.position + Camera.forward);
    }
}
