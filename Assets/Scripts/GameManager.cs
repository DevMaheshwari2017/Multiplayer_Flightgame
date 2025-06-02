using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject missileErrormsg;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    private JetController myJet;
    //private const string MissileCountKey = "activeMissiles";
    //private const int maxMissiles = 2;

    private void Awake()
    {
        missileErrormsg.SetActive(false);
        //if(!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MissileCountKey))
        //{
        //    SetMissileCount(0);
        //}
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                GameObject hitObject = result.gameObject;

                if (hitObject.CompareTag("Target"))
                {
                    Target target = hitObject.GetComponent<Target>();
                    PhotonView targetView = hitObject.GetComponent<PhotonView>();

                    if (target == null || targetView == null)
                        return;

                    if (!target.CanBeTargeted())
                    {
                        StartCoroutine(ShowTooMuchMissileMsg());
                        return;
                    }

                    // Fire missile
                    myJet.photonView.RPC("FireMissileRPC", myJet.photonView.Owner, targetView.ViewID);

                    // Increment per-target missile count
                    targetView.RPC("IncrementMissileCount", RpcTarget.AllBuffered);
                }

                break;
            }
        }
    }

    private IEnumerator ShowTooMuchMissileMsg() 
    {
        missileErrormsg.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        missileErrormsg.SetActive(false);
    }

    //private int GetMissileCount()
    //{
    //    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MissileCountKey, out object count))
    //    {
    //        Debug.Log($"The missile count is {count}");
    //        return (int)count;
    //    }
    //    return 0;
    //}

    //private void SetMissileCount(int count)
    //{
    //    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
    //    {
    //        { MissileCountKey, count }
    //    };
    //    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    //    Debug.Log($"The missile count after set is {count}");
    //}

    //[PunRPC]
    //private void RPC_DecrementMissileCount()
    //{

    //    int current = GetMissileCount();
    //    SetMissileCount(Mathf.Max(0, current - 1));

    //}

    //[PunRPC]
    //private void RPC_IncrementMissileCount()
    //{
    //    int current = GetMissileCount();
    //    if (current < maxMissiles)
    //    {
    //        SetMissileCount(current + 1);
    //    }
    //}
    public void SetMyJet(JetController jet)
    {
        myJet = jet;
    }

    [PunRPC]
    void EvaluateHitRPC(int targetViewID, float probability)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int randomVal = Random.Range(0, 10);
        bool isHit = randomVal <= probability;

        Debug.Log($"Random: {randomVal}, Prob: {probability}, Hit: {isHit}");

        photonView.RPC("ApplyHitResult", RpcTarget.AllBuffered, targetViewID, isHit);
    }

    [PunRPC]
    void ApplyHitResult(int targetViewID, bool isHit)
    {
        Debug.Log($"view id in ApplyHitResult is {targetViewID}");
        StartCoroutine(WaitAndApplyHit(targetViewID, isHit));
    }

    private IEnumerator WaitAndApplyHit(int viewID, bool isHit)
    {
        float timeout = 2f;
        float elapsed = 0f;
        GameObject targetObj = null;

        while (targetObj == null && elapsed < timeout)
        {
            PhotonView view = PhotonView.Find(viewID);
            if (view != null)
            {
                targetObj = view.gameObject;
                break;
            }

            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (targetObj != null && isHit)
        {
            targetObj.GetComponent<Target>().DeductLivesNetworked();
        }
        else if (targetObj == null)
        {
            Debug.LogWarning($"❌ Timed out waiting for PhotonView {viewID} to appear.");
        }
    }
}
