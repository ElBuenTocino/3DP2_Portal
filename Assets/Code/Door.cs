using UnityEngine;

public class Door : MonoBehaviour
{
    public float Speed;
    public Transform p1, p2;
    public bool isOpening = false;
    public bool isClosing = false;

    private void Start()
    {
        p1.parent = null;
        p2.parent = null;
    }

    private void Update()
    {
        if (isOpening)
            Open();
        else if (isClosing)
            Close();
    }

    public void Close()
    {
        transform.position = Vector3.MoveTowards(transform.position, p1.position, Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, p1.position) < 0.1f)
        {
            isClosing = false;
        }
    }

    public void Open()
    {
        transform.position = Vector3.MoveTowards(transform.position, p2.position, Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, p2.position) < 0.1f)
        {
            isOpening = false;
        }
    }

    public void StartOpen()
    {
        isOpening = true;
        isClosing = false;
    }
    public void StartClosign()
    {
        isClosing = true;
        isOpening = false;
    }
}
