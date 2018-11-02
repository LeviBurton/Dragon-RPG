using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dice
{
    public static int Roll(int numDice, int numSides)
    {
        int sum = 0;

        for (int roll = 0; roll < numDice; roll++)
        {
            // Min is inclusive lower bound.
            // Max is exclusive upper bound, so we need to add 1 to it.
            sum += Random.Range(1, numSides + 1);
        }

        return sum;
    }
}
