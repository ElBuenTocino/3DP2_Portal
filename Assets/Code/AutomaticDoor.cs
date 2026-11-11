using UnityEngine;
using UnityEngine.PlayerLoop;

public class AutomaticDoor : Door
{
    public AudioClip Door;

    private bool playerInside = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            AudioSource.PlayClipAtPoint(Door, transform.position, 1);
            Debug.Log("PlayerEntered");
            isOpening = true;
            isClosing = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && playerInside)
        {
            playerInside = false;
            AudioSource.PlayClipAtPoint(Door, transform.position, 1);
            isOpening = false;
            isClosing = true;

        }
    }
}
