using UnityEngine;

public class HeadTriggerRelay : MonoBehaviour
{
    public SnakeController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        controller?.HandleTriggerEnter(collision);
    }


}


