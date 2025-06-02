using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private int lives;
    [SerializeField] private float probabilityOfHit;

    private int activeMissiles = 0;
    public const int MaxMissiles = 2;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        RectTransform targetRect = GetComponent<RectTransform>();
        RectTransform targetContainer = GameObject.Find("TargetContainer").GetComponent<RectTransform>();

        if (targetRect != null && targetContainer != null)
        {
            targetRect.SetParent(targetContainer, false);
        }
    }
    public void DeductLivesNetworked()
    {
        lives--;
        GetComponent<Image>().color = Color.yellow;

        if (lives <= 0)
        {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null)
            {
                photonView.RPC("RPC_DestroyTarget", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    public void IncrementMissileCount()
    {
        activeMissiles++;
    }

    [PunRPC]
    public void DecrementMissileCount()
    {
        activeMissiles = Mathf.Max(0, activeMissiles - 1);
    }

    public bool CanBeTargeted()
    {
        return activeMissiles < MaxMissiles;
    }
    public float GetHitProbability()
    {
        return probabilityOfHit;
    }
    [PunRPC]
    void RPC_DestroyTarget()
    {
        Destroy(gameObject);
    }
}
