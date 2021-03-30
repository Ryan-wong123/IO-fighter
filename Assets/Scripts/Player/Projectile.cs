using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class Projectile : MonoBehaviourPun
{
    //photon view component
    public PhotonView pv;

    private void Awake()
    {
        StartCoroutine(destroyProjectileAfterCertainTime());
    }

    //method to destroy this gameobject
    [PunRPC]
    public void destoryOnNetwork()
    {
        //check is gameobject is still in the scene
        if (gameObject.activeInHierarchy == false)
        {
            //exit the function
            return;
        }
        else
        {
            //if photon view is mine
            if (pv.IsMine)
            {
                //destroy this projectile and synchronize its destroy on all client's device
                PhotonNetwork.Destroy(this.gameObject);
            }
        } 
    }

    //method to auto destroy this projectile 
    IEnumerator destroyProjectileAfterCertainTime()
    {
        //wait for 5 seconds
        yield return new WaitForSeconds(5f);

        //if photon view is mine
        if (pv.IsMine)
        {
            //destroy this projectile and synchronize its destroy on all client's device
            pv.RPC("destoryOnNetwork", RpcTarget.AllBuffered);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if this projectile hits a player
        if (other.gameObject.CompareTag("Player"))
        {
            //if photon view is not mine
            if (!pv.IsMine)
            {
                //exit
                return;
            }

            //if photon view is mine
            if (pv.IsMine)
            {
                //destroy this projectile and synchronize its destroy on all client's device
                pv.RPC("destoryOnNetwork", RpcTarget.AllBufferedViaServer);

                //add damage to the player that shoot the projectile
                //PhotonNetwork.LocalPlayer.AddScore(1);
            }
        }
    }
}
