using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public Transform m_SpawnPosition;
    public GameObject m_CompanionCube;

    public void Execute()
    {
        GameObject l_Companion = Instantiate(m_CompanionCube);
        l_Companion.transform.position = m_SpawnPosition.position;
    }
}
