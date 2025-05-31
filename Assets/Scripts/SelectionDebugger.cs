using UnityEngine;

public class SelectionDebugger : MonoBehaviour
{
    public JetController selectedJet;
    public Transform selectedTarget;

    private void OnDrawGizmos()
    {
        if (selectedJet != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(selectedJet.transform.position, 10f);
            DrawLabel(selectedJet.transform.position + Vector3.up * 40f, $"Selected Jet: {selectedJet.name}");
        }

        if (selectedTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(selectedTarget.position, 10f);
            DrawLabel(selectedTarget.position + Vector3.up * 40f, $"Selected Target: {selectedTarget.name}");
        }

        if (selectedJet != null && selectedTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(selectedJet.transform.position, selectedTarget.position);
        }
    }

    private void DrawLabel(Vector3 worldPos, string text)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(worldPos, text);
#endif
    }
}
