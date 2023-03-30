using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnap : MonoBehaviour
{
    public Transform target;
    public float snap;


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(Mathf.Round(target.position.x/snap)*snap + 0.5f, 0, Mathf.Round(target.position.z/snap)*snap + 0.5f);
        transform.position = pos;
    }
}
