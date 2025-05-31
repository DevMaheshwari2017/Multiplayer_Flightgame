using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class JetController : MonoBehaviourPun
{
    [SerializeField] private RectTransform missileSpawnPoint;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float missileMovingTime = 5;

    private bool canControl = false;


    [PunRPC]
    private void FireMissileRPC(int targetViewID)
    {
        PhotonView targetView = PhotonView.Find(targetViewID);
        if (targetView != null)
        {
            GameObject target = targetView.gameObject;
            GameObject missile = PhotonNetwork.Instantiate(missilePrefab.name, missileSpawnPoint.position, Quaternion.identity);
            
            photonView.RPC(nameof(SetMissileParent), RpcTarget.AllBuffered, missile.GetComponent<PhotonView>().ViewID);
            
            missile.GetComponent<MissileMover>().Initialize(target, missileSpawnPoint.position);
        }
    }

    public Vector3 GetMissileSpawnPosition()
    {
        return missileSpawnPoint.position;
    }

    [PunRPC]
    void SetMissileParent(int missileViewID)
    {
        PhotonView missileView = PhotonView.Find(missileViewID);
        if (missileView != null)
        {
            RectTransform missileRect = missileView.GetComponent<RectTransform>();
            if (missileRect == null) return;

            missileRect.SetParent(gameObject.transform, false);
        }
    }

    public  void EnableInput(bool enable)
    {
        canControl = enable;
    }
}
