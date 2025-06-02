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

    [PunRPC]
    void NotifyMissileDestroyed()
    {
        if (target != null)
        {
            PhotonView targetView = target.GetComponent<PhotonView>();
            if (targetView != null)
            {
                targetView.RPC("DecrementMissileCount", RpcTarget.AllBuffered);
            }
        }
    }
}
