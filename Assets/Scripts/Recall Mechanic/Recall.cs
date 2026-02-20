using UnityEngine;
using UnityEngine.InputSystem;

public class Recall : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 recallPointPosition;
    private bool RecallPointSet = false;

    // This should be linked to your Input Action (Ctrl)
    public void Teleport(InputAction.CallbackContext context)
    {
        if (context.started == false)
        {
            return;
        }

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.Find("Player(Clone)");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        if (RecallPointSet == false)
        {
            recallPointPosition = playerTransform.position;
            RecallPointSet = true;
        }
        else
        {
            playerTransform.position = recallPointPosition;
        }
    }
}
