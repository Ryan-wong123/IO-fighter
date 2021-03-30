using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{
    private void Awake()
    {
        spawnPlayer();
    }

    private void spawnPlayer()
    {
        PhotonNetwork.Instantiate("PlayerCharacter", new Vector3(96, 0, 77), gameObject.transform.rotation);
    }
}