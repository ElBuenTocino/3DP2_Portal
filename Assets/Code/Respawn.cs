using System;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public Transform point;
    public Material InactiveMaterial, ActiveMaterial;

    public static event Action<RespawnPoint> OnChangeRespawn;

    public AudioClip Checkpoint;

    private void Start()
    {
        if (point == null)
        {
            point = transform;
        }
        else
        {
            point.parent = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.GetComponent<PlayerRespawn>() != null)
        {
            OnChangeRespawn?.Invoke(this);
            if (Checkpoint != null)
            {
                AudioSource.PlayClipAtPoint(Checkpoint, point.position, 30f);
            }
            if (ActiveMaterial != null && InactiveMaterial != null && gameObject.GetComponent<MeshRenderer>() != null)
            {
                gameObject.GetComponent<MeshRenderer>().material = ActiveMaterial;
            }
        }
    }
}
