using UnityEngine;

public class Turret : MonoBehaviour
{
    public LineRenderer m_Laser;
    public float m_MaxDistance;
    public LayerMask m_LaserLayerMask;
    public float m_MaxValidAngle = 40.0f;
    public float m_TeleportPortalOffset = 1.2f;
    Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 l_Position = Vector3.forward * m_MaxDistance;

        Ray l_Ray = new Ray(m_Laser.transform.position, m_Laser.transform.forward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_LaserLayerMask, QueryTriggerInteraction.Ignore))
        {
            l_Position = Vector3.forward * l_RaycastHit.distance;

            if (l_RaycastHit.collider.GetComponent<PlayerStats>() != null)
            {
                l_RaycastHit.collider.GetComponent<PlayerStats>().Die();
            }
        }
        m_Laser.SetPosition(1, l_Position);

        //if (transform.up)

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
        Vector3 l_Direction = m_Rigidbody.linearVelocity.normalized;
        float l_DotAngle = Vector3.Dot(-l_Direction, _Portal.transform.forward);
        return l_DotAngle >= l_MaxDotAngle;
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_Direction = m_Rigidbody.linearVelocity.normalized;
        Vector3 l_WorldPosition = transform.position + l_Direction * m_TeleportPortalOffset;
        Vector3 l_WorldForward = transform.forward;
        Vector3 l_WorldLinearVelocity = m_Rigidbody.linearVelocity;
        Vector3 l_localPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition);
        Vector3 l_LocalForward = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldForward);
        Vector3 l_LocalLinearVelocity = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldLinearVelocity);

        float l_Scale = _Portal.m_MirrorPortal.transform.localScale.x / _Portal.transform.localScale.x;
        m_Rigidbody.isKinematic = true;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_localPosition);
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        l_Direction = Vector3.zero;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.linearVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalLinearVelocity);
        transform.localScale = Vector3.one * transform.localScale.x * l_Scale;
    }
}