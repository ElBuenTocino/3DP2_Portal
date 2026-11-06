using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class FPSController : MonoBehaviour
{
    public Transform m_PitchController;
    float m_Yaw;
    float m_Pitch;
    public Vector3 m_Direction;
    public float m_Speed;
    public float m_SprintSpeed;
    public float maxPitch;
    public float minPitch;
    public float m_YawSpeed;
    public float m_PitchSpeed;
    public float m_JumpSpeed;
    public bool m_InvertYaw;
    public bool m_InvertPitch;
    CharacterController m_CharacterController;
    private float m_VerticalSpeed = 0f;

    private bool deathHandled;
    public AudioClip Death;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_GrabOjectKeyCode = KeyCode.E;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;

    public bool m_AngleLocked = false;
    public bool m_AimLocked = false;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockCursorCode = KeyCode.O;

    [Header("Shoot")]
    public Camera m_Camera;
    public float m_MaxShootDistance = 200.0f;
    public LayerMask m_ShootLayerMask;
    public int m_LeftShootButton = 0;
    public int m_RightShootButton = 1;
    public bool CanShootBool;
    public AudioClip ShootSound;

    [Header("Animations")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;

    [Header("Extra")]
    public bool isAlive;

    [Header("Portals")]
    public Portal BluePortal, OrangePortal;
    public float m_TeleportPortalOffset = 1.2f;
    public float m_MaxValidAngle = 40.0f;

    [Header("Grab")]
    public Transform m_GrabTransform;
    bool m_AttachingObject;
    bool m_ObjectAttached;
    Rigidbody m_GrabObject;
    float m_GrabTime;
    public float m_TimeToGrab = 0.8f;
    public float m_ShootAttachedObjectForce = 20.0f;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    //private void OnEnable()
    //{
    //    //PlayerStats.OnDeath += SetDeath;
    //    PlayerStats.OnDeath += Kill;
    //    CanvasManager.OnRetry += SetLife;
    //    ShootingGallery.OnFinishGallery += IsAbleToMove;
    //    GameController.GetGameController()?.SetPlayer(this);

    //}

    //private void OnDisable()
    //{
    //    //PlayerStats.OnDeath -= SetDeath;
    //    PlayerStats.OnDeath -= Kill;
    //    CanvasManager.OnRetry -= SetLife;
    //    GameController.GetGameController()?.SetPlayer(null);
    //}

    private void Start()
    {
        isAlive = true;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = transform.rotation.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        //SetIdleWeaponAnimation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockCursorCode))
        {
            Cursor.lockState = Cursor.lockState != CursorLockMode.Locked ? CursorLockMode.Locked : CursorLockMode.None;
            /*if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;*/
        }

        if (isAlive)
        {
            float l_MouseX = Input.GetAxis("Mouse X");
            float l_MouseY = Input.GetAxis("Mouse Y");

            if (m_InvertPitch)
            {
                l_MouseY *= -1;
            }

            if (m_InvertYaw)
            {
                l_MouseY *= -1;
            }

            if (!m_AngleLocked)
            {
                m_Yaw = m_Yaw + l_MouseX * m_YawSpeed * Time.deltaTime;
                m_Pitch = m_Pitch + l_MouseY * m_PitchSpeed * Time.deltaTime;
            }

            m_Pitch = Mathf.Clamp(m_Pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(0f, m_Yaw, 0f);
            m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0f, 0f);

            m_Direction = Vector3.zero;
            Vector3 l_Right = transform.right;
            Vector3 l_Forward = transform.forward;

            l_Right.y = 0f;
            l_Forward.y = 0f;
            l_Right.Normalize();
            l_Forward.Normalize();

            if (Input.GetKey(m_RightKeyCode))
                m_Direction += transform.right;
            else if (Input.GetKey(m_LeftKeyCode))
                m_Direction -= transform.right;

            if (Input.GetKey(m_UpKeyCode))
                m_Direction += transform.forward;
            else if (Input.GetKey(m_DownKeyCode))
                m_Direction -= transform.forward;

            float l_Speed = m_Speed;
            if (Input.GetKey(m_RunKeyCode))
                l_Speed = m_SprintSpeed;
            if (Input.GetKeyDown(m_JumpKeyCode) && m_VerticalSpeed == 0f)
                m_VerticalSpeed = m_JumpSpeed;

            m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
            m_Direction.Normalize();
            Vector3 l_Direction = m_Direction * l_Speed * Time.deltaTime;
            l_Direction.y = m_VerticalSpeed * Time.deltaTime;
            CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Direction);
            if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
                m_VerticalSpeed = 0.0f;
            else if ((l_CollisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f)
                m_VerticalSpeed = 0f;

            if (CanGrabObject())
            {
                GrabObject();
            }

            if (CanShoot() && MustShoot(out Portal portal))
                Shoot(portal);

            UpdateAttachingObject();

            UpdateAttachedObject();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    bool CanGrabObject()
    {
        if (Input.GetKeyDown(m_GrabOjectKeyCode) && !m_AttachingObject && !m_ObjectAttached)
        {
            return true;
        }
        return false;
    }

    void GrabObject()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value, QueryTriggerInteraction.Ignore))
        {
            if (l_RaycastHit.collider.CompareTag("Cube"))
            {
                GrabObject(l_RaycastHit.collider);
            }
        }
    }

    void UpdateAttachingObject()
    {
        if (m_AttachingObject)
        {
            float l_DiffTime = Time.time - m_GrabTime;
            float l_Pct = Mathf.Min(1.0f, l_DiffTime / m_TimeToGrab);
            m_GrabObject.transform.position = Vector3.Lerp(m_GrabObject.transform.position, m_GrabTransform.position, l_Pct);
            m_GrabObject.transform.rotation = Quaternion.Lerp(m_GrabObject.transform.rotation, m_GrabTransform.rotation, l_Pct);
            if (l_Pct >= 0.9f)
            {
                m_AttachingObject = false;
                m_ObjectAttached = true;
            }
        }
    }
    void UpdateAttachedObject()
    {
        if (m_ObjectAttached)
        {
            if (Input.GetKeyDown(m_GrabOjectKeyCode))
            {
                ThrowAttachedObject(0.0f);
            }
            else if (Input.GetMouseButtonDown(m_LeftShootButton))
            {
                ThrowAttachedObject(m_ShootAttachedObjectForce);
            }
        }
    }

    void ThrowAttachedObject(float Force)
    {
        m_GrabObject.transform.SetParent(null);
        m_GrabObject.isKinematic = false;
        m_GrabObject.linearVelocity = m_GrabTransform.transform.forward * Force;
        m_ObjectAttached = false;
    }

    void GrabObject(Collider _collider)
    {
        m_GrabObject = _collider.GetComponent<Rigidbody>();
        m_GrabObject.isKinematic = true;
        m_AttachingObject = true;
        m_GrabObject.transform.SetParent(m_GrabTransform);
        m_GrabTime = Time.time;
    }

    bool CanShoot()
    {
        if (!m_ObjectAttached && !m_AttachingObject)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetCanShoot(int f)
    {
        if (f == 0)
        {
            CanShootBool = false;
        }
        else
        {
            CanShootBool = true;
        }
    }

    public void IsAbleToMove(bool s)
    {
        isAlive = s;
        if (isAlive)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    bool MustShoot(out Portal portal)
    {
        if (Input.GetMouseButtonDown(m_LeftShootButton))
        {
            portal = BluePortal;
            return true;
        }
        else if (Input.GetMouseButtonDown(m_RightShootButton))
        {
            portal = OrangePortal;
            return true;
        }
        portal = null;
        return false;
    }

    void Shoot(Portal _Portal)
    {
        if (_Portal == null)
        {
            return;
        }
        //SetShootWeaponAnimation();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
        {
            if (_Portal.IsValidPosition(l_RaycastHit.point, l_RaycastHit.normal))
            {
                _Portal.gameObject.SetActive(true);
                _Portal.m_CrosshairPortal.FillCrosshair(true);
            }
            else
            {
                _Portal.gameObject.SetActive(false);
                _Portal.m_CrosshairPortal.FillCrosshair(false);
            }

        }
        else
        {
            _Portal.gameObject.SetActive(false);
            _Portal.m_CrosshairPortal.FillCrosshair(false);
        }
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
        else if (other.GetComponent<CompanionSpawner>() != null)
        {
            other.GetComponent<CompanionSpawner>().Execute();
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

        m_CharacterController.enabled = false;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_localPosition);
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Direction = Vector3.zero;
        m_CharacterController.enabled = true;
    }


    void SetIdleWeaponAnimation()
{
    m_Animation.CrossFade(m_IdleAnimationClip.name, 0.1f);
}

void SetShootWeaponAnimation()
{
    m_Animation.CrossFade(m_ShootAnimationClip.name);
    m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.3f);
}
void SetDeath()
    {
        
        Debug.Log("He muerto");
        //playerSoundControler.playDie();
        Cursor.lockState = CursorLockMode.Confined;
        isAlive = false;
    }

    public void Kill()
    {
        //if (!deathHandled)
        //{
        //    deathHandled = true;
        //    AudioSource.PlayClipAtPoint(Death, transform.position, 1);
        //    Cursor.lockState = CursorLockMode.None;
        //    StartCoroutine(GameController.GetGameController().GetFade().FadeIn(() =>
        //    {
        //        GameController.GetGameController().GetFade().gameObject.SetActive(false);
        //        // GameController.GetGameController().Restart();
        //    }));
        //}
    }

    void SetLife()
    {
        deathHandled = false;
        Cursor.lockState = CursorLockMode.Locked;
        isAlive = true;
    }
}
