using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target = null;
    void Update()
    {
        if (target == null)
            return;

        float z = this.transform.position.z;
        Vector3 position = this.target.position;
        position.z = z;
        this.transform.position = position;
    }
}
