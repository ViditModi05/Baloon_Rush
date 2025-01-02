﻿using UnityEngine;

public class LevelGenerator : Singleton<LevelGenerator>
{
    #region Variables

    [SerializeField] LevelData[] levelDatas;
    [SerializeField] private Material _skyMaterial;
    [SerializeField] private Material _waterMaterial;

    LevelData currentLevelData;

    #endregion

    #region Other Methods

    public void SpawnLevel(int index)
    {
        if (index > levelDatas.Length - 1)
            index = GetRandomLevel();

        currentLevelData = levelDatas[index];
        _skyMaterial.color = currentLevelData.skyboxColor;
        _waterMaterial.color = currentLevelData.waterColor;
        Instantiate(currentLevelData.levelPrefab);
        PlayerPrefs.SetInt("LevelIndex", index);
    }

    int GetRandomLevel()
    {
        int index = Random.Range(0, levelDatas.Length);

        int lastLevel = PlayerPrefs.GetInt("LevelIndex");
        if (index == lastLevel)
            return GetRandomLevel();
        else
            return index;
    }

    #endregion
}
