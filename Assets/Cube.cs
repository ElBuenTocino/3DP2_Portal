using UnityEngine;

public class Cube : MonoBehaviour
{
    public Vector3 m_Direction;
    public float m_MaxValidAngle = 40.0f;
    public float m_TeleportPortalOffset = 1.2f;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Portal>() != null)
        {
            var l_Portal = (Portal)other.GetComponent<Portal>();
            if (CanTeleport(l_Portal))
            {
                Teleport(l_Portal);
            }
        }
    }

    bool CanTeleport(Portal _Portal)
    {
        float l_MaxDotAngle = Mathf.Cos(m_MaxValidAngle * Mathf.Deg2Rad);
        float l_DotAngle = Vector3.Dot(-m_Direction, _Portal.transform.forward);
        return l_DotAngle >= l_MaxDotAngle;
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_WorldPosition = transform.position + m_Direction * m_TeleportPortalOffset;
        Vector3 l_WorldForward = transform.forward;
        Vector3 l_localPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition);
        Vector3 l_LocalForward = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldForward);

        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_localPosition);
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        m_Direction = Vector3.zero;
    }
}
