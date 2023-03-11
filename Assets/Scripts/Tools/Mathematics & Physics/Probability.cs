using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public static class Probability
{
    public static void InitialiseProbabilityDistribution(this IProbability[] probabilities)
    {
        probabilities.NormaliseSpawnRates();
        probabilities.InitialiseCumulativeSpawnRates();
    }

    private static void NormaliseSpawnRates(this IProbability[] probabilities)
    {
        float total = 0;

        for (int i = 0; i < probabilities.Length; i++)
        {
            total += probabilities[i].chance;
        }
        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i].chance /= total;
        }
    }

    private static void InitialiseCumulativeSpawnRates(this IProbability[] probabilities)
    {
        for (int i = 1; i < probabilities.Length; i++)
        {
            probabilities[i].chance += probabilities[i - 1].chance;
        }
    }
}

public interface IProbability
{
    [ShowInInspector]
    float chance { get; set; }
}