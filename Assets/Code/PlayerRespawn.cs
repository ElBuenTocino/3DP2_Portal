using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public RespawnPoint CurrentRespawn;
    

    private void OnEnable()
    {
        RespawnPoint.OnChangeRespawn += ChangeRespawn;
        //CanvasManager.OnRetry += RespawnPlayer;
    }

    private void OnDisable()
    {
        RespawnPoint.OnChangeRespawn -= ChangeRespawn;
        //CanvasManager.OnRetry += RespawnPlayer;
    }

    private void Start()
    {
        if (CurrentRespawn != null)
        {
            gameObject.transform.position = CurrentRespawn.transform.position;
        }
    }

    void ChangeRespawn(RespawnPoint respawn)
    {
        CurrentRespawn = respawn;
    }

    public void RespawnPlayer()
    {
        if (CurrentRespawn != null)
        {
            gameObject.transform.position = CurrentRespawn.point.position;
        }
        Physics.SyncTransforms();
    }
}
