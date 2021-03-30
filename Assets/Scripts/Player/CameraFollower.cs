using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    GameObject playerCamera;
    GameObject player;

    public float verticalOffset = 50f;
    public float horizontalOffset = 2f;

    public float camDegress = 80;
    Vector3 cameraOffset;


    // Start is called before the first frame update
    void Start()
    {
        playerCamera = this.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        cameraOffset = new Vector3(0, verticalOffset, -horizontalOffset);

        playerCamera.transform.Rotate(camDegress, 0, 0, Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }
        else
        {
            playerCamera.transform.position = player.transform.position + cameraOffset;
        }
    }
}
