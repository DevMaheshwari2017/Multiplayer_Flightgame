using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class JeyUIFixer : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        StartCoroutine(AttachToUI(info.Sender));
    }
    private IEnumerator AttachToUI(Player owner)
    {
        yield return null; // Wait 1 frame

        SpawnManager spawnManager = FindAnyObjectByType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager not found!");
            yield break;
        }

        spawnManager.AttachJetToUI(this.gameObject, owner);
    }
}
