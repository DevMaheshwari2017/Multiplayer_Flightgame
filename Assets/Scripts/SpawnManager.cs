using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [Space(5)]
    [Header("Jet")]
    [SerializeField] private GameObject jetPrefab;
    [SerializeField] private RectTransform[] spawnPoints;
    [SerializeField] private RectTransform jetContainer;

    [Space(5)]
    [Header("Target")]
    [SerializeField] private RectTransform[] targetSpawnPoints;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private RectTransform targetContainer;
     
    [Space(5)]
    [Header("Script Ref")]
    [SerializeField] private GameManager gameManager;


    private void Awake()
    {
        SpawnTargets();
        SpawnPlayerJet();
    }

    private void SpawnPlayerJet()
    {
        Debug.Log("Spawning player");
        Player[] sortedPlayers = PhotonNetwork.PlayerList; // sorted by join order
        int playerIndex = System.Array.IndexOf(sortedPlayers, PhotonNetwork.LocalPlayer);

        if (playerIndex >= spawnPoints.Length)
        {
            Debug.LogError("No spawn point available for this player!");
            return;
        }

        // Instantiate the Jet
        GameObject jet = PhotonNetwork.Instantiate(jetPrefab.name, Vector3.zero, Quaternion.identity);
    }

    private void SpawnTargets() 
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var spawnPoint in targetSpawnPoints)
            {

                // Convert UI anchoredPosition to world position
                Vector3 worldPos = spawnPoint.TransformPoint(spawnPoint.anchoredPosition);

                // Instantiate via Photon
                GameObject targetGO = PhotonNetwork.Instantiate(targetPrefab.name, worldPos, Quaternion.identity);

                // Attach to canvas & reset UI transform
                RectTransform targetRect = targetGO.GetComponent<RectTransform>();
                targetRect.SetParent(targetContainer, false);

                // Match UI properties
                targetRect.anchorMin = spawnPoint.anchorMin;
                targetRect.anchorMax = spawnPoint.anchorMax;
                targetRect.pivot = spawnPoint.pivot;
                targetRect.anchoredPosition = spawnPoint.anchoredPosition;
                targetRect.localRotation = spawnPoint.localRotation;
                targetRect.localScale = spawnPoint.localScale;
            }
        }
    }

    public void AttachJetToUI(GameObject jet, Player owner)
    {
        // Find player index in join order
        Player[] sortedPlayers = PhotonNetwork.PlayerList;
        int playerIndex = System.Array.IndexOf(sortedPlayers, owner);

        if (playerIndex >= spawnPoints.Length)
        {
            Debug.LogError("Not enough spawn points for player: " + owner.NickName);
            return;
        }

        RectTransform jetRect = jet.GetComponent<RectTransform>();
        RectTransform spawnPoint = spawnPoints[playerIndex];

        if (jetRect == null || spawnPoint == null)
        {
            Debug.LogError("Missing RectTransform");
            return;
        }


        // Set parent
        jetRect.SetParent(jetContainer, false);

        // ✅ Match anchor & pivot
        jetRect.anchorMin = spawnPoint.anchorMin;
        jetRect.anchorMax = spawnPoint.anchorMax;
        jetRect.pivot = spawnPoint.pivot;

        // ✅ Now safely set position, rotation, and scale
        jetRect.anchoredPosition = spawnPoint.anchoredPosition;
        jetRect.localRotation = spawnPoint.localRotation;
        jetRect.localScale = spawnPoint.localScale;

        // Only local player sets reference to GameManager
        if (owner == PhotonNetwork.LocalPlayer)
        {
            gameManager.SetMyJet(jet.GetComponent<JetController>());
        }
    }
}
