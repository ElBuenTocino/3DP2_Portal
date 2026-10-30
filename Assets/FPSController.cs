using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public Transform m_PitchController;
    float m_Yaw;
    float m_Pitch;
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
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;

    public bool m_AngleLocked = false;
    public bool m_AimLocked = false;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockCursorCode = KeyCode.O;

    [Header("Shoot")]
    public Camera m_Camera;
    public float m_MaxShootDistance = 200.0f;
    public LayerMask m_ShootLayerMask;
    public int m_ShootButton = 0;
    public bool CanShootBool;
    public AudioClip ShootSound;

    [Header("Animations")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;

    [Header("Extra")]
    public bool isAlive;

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

            Vector3 l_Direction = Vector3.zero;
            Vector3 l_Right = transform.right;
            Vector3 l_Forward = transform.forward;

            l_Right.y = 0f;
            l_Forward.y = 0f;
            l_Right.Normalize();
            l_Forward.Normalize();

            if (Input.GetKey(m_RightKeyCode))
                l_Direction += transform.right;
            else if (Input.GetKey(m_LeftKeyCode))
                l_Direction -= transform.right;

            if (Input.GetKey(m_UpKeyCode))
                l_Direction += transform.forward;
            else if (Input.GetKey(m_DownKeyCode))
                l_Direction -= transform.forward;

            float l_Speed = m_Speed;
            if (Input.GetKey(m_RunKeyCode))
                l_Speed = m_SprintSpeed;
            if (Input.GetKeyDown(m_JumpKeyCode) && m_VerticalSpeed == 0f)
                m_VerticalSpeed = m_JumpSpeed;

            m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;

            l_Direction.Normalize();
            l_Direction = l_Direction * l_Speed * Time.deltaTime;
            l_Direction.y = m_VerticalSpeed * Time.deltaTime;
            CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Direction);
            if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
                m_VerticalSpeed = 0.0f;
            else if ((l_CollisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f)
                m_VerticalSpeed = 0f;

            //if (CanShoot() && MustShoot())
            //    Shoot();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    bool CanShoot()
    {
        return CanShootBool;
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

    //bool MustShoot()
    //{
    //    if (GetComponent<GunBehaviour>().CurrentBullets >= 1)
    //    {
    //        return Input.GetMouseButtonDown(m_ShootButton);
    //    }
    //    else return false;
    //}

    //void Shoot()
    //{
    //    MuzzleParticle.Play();
    //    AudioSource.PlayClipAtPoint(ShootSound, transform.position,0.2f);
    //    GetComponent<GunBehaviour>().Shoot();
    //    SetShootWeaponAnimation();
    //    Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
    //    if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask, QueryTriggerInteraction.Ignore))
    //    {
    //        if (l_RaycastHit.transform.gameObject.GetComponent<Target>() != null)
    //        {
    //            l_RaycastHit.transform.gameObject.GetComponent<Target>().GetHit();
    //        }
    //        else if (l_RaycastHit.collider.GetComponent<HitCollider>() != null)
    //        {
    //            l_RaycastHit.collider.GetComponent<HitCollider>().Hit();
    //        }
    //        else
    //        {
    //            CreateShootParticles(l_RaycastHit.point, l_RaycastHit.normal);
    //        }

    //    }
    //}

    //void SetIdleWeaponAnimation()
    //{
    //    m_Animation.CrossFade(m_IdleAnimationClip.name, 0.1f);
    //}

    //void SetShootWeaponAnimation()
    //{
    //    m_Animation.CrossFade(m_ShootAnimationClip.name);
    //    m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.3f);
    //}
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
