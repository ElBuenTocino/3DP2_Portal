using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController m_Instance = null;

    public KeyCode m_RestartKeyCode = KeyCode.F5;
    public FPSController m_Player;
    Fade m_Fade;

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    static public GameController GetGameController()
    {
        return m_Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_RestartKeyCode))
        {
            Restart();
        }
    }

    public void SetPlayer(FPSController player)
    {
        m_Player = player;
    }

    public FPSController GetPlayer()
    {
        return m_Player;
    }

    public void Restart()
    {
        m_Player.GetComponent<PlayerRespawn>().RespawnPlayer();
        StartCoroutine(m_Fade.FadeOut());

    }

    public void SetFade(Fade _Fade)
    {
        m_Fade = _Fade;
    }

    public Fade GetFade()
    {
        return m_Fade;
    }
}
