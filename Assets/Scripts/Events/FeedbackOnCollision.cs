using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class FeedbackOnCollision : MonoBehaviour
{
    [SerializeField] private MMF_Player feedback;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        feedback?.PlayFeedbacks();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            feedback?.PlayFeedbacks();
    }
}