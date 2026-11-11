using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    public UnityEvent m_EventEnter;
    public UnityEvent m_EventExit;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.GetComponent<CompanionCube>() != null)
        {
            m_EventEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.GetComponent<CompanionCube>() != null)
        {
            m_EventExit.Invoke();
        }
    }
}
