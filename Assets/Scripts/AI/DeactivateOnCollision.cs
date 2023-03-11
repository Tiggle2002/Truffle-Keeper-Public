using UnityEngine;

public class DeactivateOnCollision : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Animal"))
        {
            StartCoroutine(collision.gameObject.DespawnCoroutine(1.5f));
        }
    }
}