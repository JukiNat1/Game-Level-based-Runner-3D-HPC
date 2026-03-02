using UnityEngine;

/// <summary>
/// Gắn vào GameObject cổng đích (FinishGate).
/// Dùng BoxCollider với Is Trigger = true để phát hiện Player chạm vào.
/// </summary>
public class FinishLine : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (GoalManager.Instance != null)
            {
                GoalManager.Instance.TriggerVictory();
            }
            else
            {
                Debug.LogError("FinishLine: GoalManager.Instance is null! " +
                               "Hãy đảm bảo GoalManager đã được đặt vào Scene.");
            }
        }
    }

    /// <summary>
    /// Vẽ vùng trigger trong Unity Editor để dễ căn chỉnh.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(bc.center, bc.size);
        }
    }
}
