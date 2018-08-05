using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    [SerializeField] Transform characterPrefab;
    [SerializeField] Transform spawnPoint;

    public void OnClick()
    {
        var characterTransform = Instantiate(characterPrefab);
        characterTransform.transform.position = spawnPoint.position;
    }
}
