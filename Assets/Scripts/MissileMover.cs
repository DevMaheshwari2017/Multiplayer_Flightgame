using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MissileMover : MonoBehaviourPun
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float duration = 5f;
    private float elapsedTime;

    private bool isMoving = false;
    private GameObject target;
    public void Initialize(GameObject _target, Vector3 Startpos)
    {
        target = _target;
        this.targetPos = _target.transform.position;
        this.startPos = Startpos;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        transform.position = Vector3.Lerp(startPos, targetPos, t);

        if (t >= 1f)
        {
            isMoving = false;
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"gameobject hit is {collision.gameObject}");
        if (collision.gameObject.CompareTag("Target") && collision.gameObject == target)
        {
            PhotonView targetView = collision.gameObject.GetComponent<PhotonView>();
            if (targetView != null)
            {
                int viewID = targetView.ViewID;
                Debug.Log($"Collision obj: {collision.gameObject.name} View id is {collision.gameObject.GetComponent<PhotonView>().ViewID}:{viewID}");
                float probability = target.GetComponent<Target>().GetHitProbability();
                PhotonView.Get(this).RPC("NotifyMissileDestroyed", RpcTarget.All);
                FindAnyObjectByType<GameManager>().photonView.RPC("EvaluateHitRPC", RpcTarget.MasterClient, viewID, probability);
                StartCoroutine(DestroyMissileAfterDelay());
            }
        }
    }

    private IEnumerator DestroyMissileAfterDelay()
    {
        yield return new WaitForSeconds(1f); // enough time for RPCs to arrive
        PhotonNetwork.Destroy(gameObject);
    }
    //[PunRPC]
    //void EvaluateHitRPC(int targetViewID, float probability)
    //{
    //    if (!PhotonNetwork.IsMasterClient) return;

    //    Debug.Log($"view id in EvaluateHitRPC is {targetViewID}");
    //    int randomVal = Random.Range(0, 10);
    //    bool isHit = randomVal <= probability;

    //    Debug.Log($"Random: {randomVal}, Prob: {probability}, Hit: {isHit}");

    //    photonView.RPC("ApplyHitResult", RpcTarget.AllBuffered, targetViewID, isHit);
    //}

    //[PunRPC]
    //void ApplyHitResult(int targetViewID, bool isHit)
    //{
    //    Debug.Log($"view id in ApplyHitResult is {targetViewID}");
    //    StartCoroutine(WaitAndApplyHit(targetViewID, isHit));
    //}
    //private IEnumerator WaitAndApplyHit(int viewID, bool isHit)
    //{
    //    float timeout = 2f;  // Max wait time
    //    float elapsed = 0f;
    //    GameObject targetObj = null;

    //    while (targetObj == null && elapsed < timeout)
    //    {
    //        PhotonView view = PhotonView.Find(viewID);
    //        if (view != null)
    //        {
    //            targetObj = view.gameObject;
    //            break;
    //        }

    //        elapsed += 0.1f;
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    if (targetObj != null && isHit)
    //    {
    //        targetObj.GetComponent<Target>().DeductLivesNetworked();
    //    }
    //    else if (targetObj == null)
    //    {
    //        Debug.LogWarning($"❌ Timed out waiting for PhotonView {viewID} to appear.");
    //    }
    //}

    [PunRPC]
    void NotifyMissileDestroyed()
    {
        FindAnyObjectByType<GameManager>().photonView.RPC("RPC_DecrementMissileCount", RpcTarget.All);
    }
}
