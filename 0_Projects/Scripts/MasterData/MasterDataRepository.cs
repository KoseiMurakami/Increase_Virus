/*#==========================================================================#*/
/*#    MasterDataRepository                                                  #*/
/*#                                                                          #*/
/*#    Summary    :    マスターデータの配置とロード                          #*/
/*#                                                                          #*/
/*#==========================================================================#*/
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MasterDataRepository : ScriptableObject
{
    [SerializeField]
    private MstData mstData = default;

    public void GetMstData_Enemy(out List<EnemyTable> enemyTableList)
    {
        enemyTableList = mstData.EnemyTableList;
    }

    public void GetMstData_Item(out List<ItemTable> itemTableList)
    {
        itemTableList = mstData.ItemTableList;
    }

    public void GetMstData_Virus(out List<VirusTable> virusTableList)
    {
        virusTableList = mstData.VirusTableList;
    }
}