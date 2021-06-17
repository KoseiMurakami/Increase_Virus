using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class MstData : ScriptableObject
{
	public List<EnemyTable> EnemyTableList;
	public List<ItemTable> ItemTableList;
    public List<VirusTable> VirusTableList;
}

[System.Serializable]
public class EnemyTable
{
    public int Id;
    public string Name;
    public string AssetName;
    public float Hp;
    public int Time;
    public int MedicineLevel;
    public int Level;
    public string EnemyText;
}

[System.Serializable]
public class ItemTable
{
    public int Id;
    public string Name;
    public string AssetName;
    public int Price;
    public string Text;
}

[System.Serializable]
public class VirusTable
{
    public int Id;
    public string Name;
    public string AssetName;
    public float Hp;
    public float Attack;
    public float Increase;
}

public enum RESULT_KIND
{
    YOU_WIN,
    YOU_ALIVE,
    YOU_LOSE
}