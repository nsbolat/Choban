using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minmapCamera : MonoBehaviour
{
    public GameObject player;

    public Vector3 offset;
    // Start is called before the first frame update
    private void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition + offset;

        //transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
    }
}
