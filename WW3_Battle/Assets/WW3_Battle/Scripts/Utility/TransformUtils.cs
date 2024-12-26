using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{
    public static List<GameObject> UpdateSpawnedObjects<T>(
        List<T> serverDataList,
        List<GameObject> spawnedObjects,
        GameObject prefab,
        Transform parentTransform)
    {
        int diff = serverDataList.Count - spawnedObjects.Count;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                GameObject newObj = GameObject.Instantiate(prefab, parentTransform);
                spawnedObjects.Add(newObj);
            }
        }
        else if (diff < 0)
        {
            for (int i = 0; i < -diff; i++)
            {
                GameObject objToRemove = spawnedObjects[spawnedObjects.Count - 1];
                GameObject.Destroy(objToRemove);
                spawnedObjects.RemoveAt(spawnedObjects.Count - 1);
            }
        }

        return spawnedObjects;
    }
}