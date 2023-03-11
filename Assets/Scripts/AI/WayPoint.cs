using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [SerializeField] private AttractantID attractantID;

    public bool IsAttractant(AttractantID[] attractants)
    {
        for (int i = 0; i < attractants.Length; i++)
        {
            if (attractants[i] == attractantID)
            {
                return true;
            }
        }
        return false;
    }
}
