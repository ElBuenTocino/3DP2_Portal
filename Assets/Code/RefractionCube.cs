using UnityEngine;

public class RefractionCube : CompanionCube
{
    public LineRenderer m_Laser;
    bool m_DisableLaserOnNextFrame = true;
    public float m_MaxDistance;
    public LayerMask m_LaserLayerMask;

    private void Update()
    {
        if (m_DisableLaserOnNextFrame)
        {
            m_Laser.gameObject.SetActive(false);
        }

        m_DisableLaserOnNextFrame = true;
    }

    public void Refract()
    {
        if (!m_DisableLaserOnNextFrame)
        {
            return;
        }

        m_DisableLaserOnNextFrame = false;

        // Same as turret
        Vector3 l_Position = Vector3.forward * m_MaxDistance;

        Ray l_Ray = new Ray(m_Laser.transform.position, m_Laser.transform.forward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_LaserLayerMask, QueryTriggerInteraction.Ignore))
        {
            l_Position = Vector3.forward * l_RaycastHit.distance;

            if (l_RaycastHit.collider.GetComponent<RefractionCube>() != null)
            {
                l_RaycastHit.collider.GetComponent<RefractionCube>().Refract();
            }

            if (l_RaycastHit.collider.GetComponent<PlayerStats>() != null)
            {
                l_RaycastHit.collider.GetComponent<PlayerStats>().Die();
            }
        }
        m_Laser.gameObject.SetActive(true);
        m_Laser.SetPosition(1, l_Position);
    }
}
