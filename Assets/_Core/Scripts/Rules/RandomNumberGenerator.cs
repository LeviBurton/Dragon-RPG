using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomNumberGenerator
{
    public static int randomSeed = 0;

    // we should only do this if we can at some point serialize the state of the random number generator,
    // otherwise, every time we load the game we will get the same sequence of numbers.
    public static void SetRandomSeed(int newRandomSeed)
    {
        randomSeed = newRandomSeed;
        Random.InitState(newRandomSeed);
    }
}
