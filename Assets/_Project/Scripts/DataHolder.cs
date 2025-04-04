using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DataHolder", menuName = "AllData")]
public class DataHolder : ScriptableObject
{
    public Sprite[] SightImages;
    public Sprite[] MagImages;
    public Sprite[] BarrelImages;
    public Sprite[] TacticalImages;
    public Sprite[] StockImages;
    public string[] SightNames;
    public string[] MagNames;
    public string[] BarrelNames;
    public string[] TacticalNames;
    public string[] StockNames;
    #region singleton
    private static DataHolder instance;
    public static DataHolder Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load("SO/Data") as DataHolder;
            return instance;
        }
    }
    #endregion
}