using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private int lives;
    [SerializeField] private float probabilityOfHit;

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
