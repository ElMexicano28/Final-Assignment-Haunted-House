using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform player;
    bool m_IsPlayerInRange;

    public GameEnding gameEnding;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = false;
        }
    }

    void Update()
    {
        if (m_IsPlayerInRange)
        {
            Vector3 direction = player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                // handles child colliders
                var playerMovement = raycastHit.collider.GetComponentInParent<PlayerMovement>();
                if (playerMovement == null)
                {
                    // if ray directly hit assigned player transform
                    if (raycastHit.collider.transform == player)
                    {
                        // try to get PlayerMovement from the player Transform
                        playerMovement = player.GetComponent<PlayerMovement>();
                    }
                }

                if (playerMovement != null)
                {
                    // Ask player if this hit should registe, TryRegisterHit handles cooldown and shield consumption
                    bool shouldBeCaught = playerMovement.TryRegisterHit();
                    if (shouldBeCaught)
                    {
                        gameEnding.CaughtPlayer();
                    }
                }
            }
        }
    }
}