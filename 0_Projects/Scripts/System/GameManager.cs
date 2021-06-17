using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public int battleId { set; get; }
    public int unlockCount { set; get; }

    private MasterDataRepository masterDataRepository;

    private Dictionary<int, RESULT_KIND> gameTable = new Dictionary<int, RESULT_KIND>();

    private List<EnemyTable> enemyTableList = new List<EnemyTable>();
    private List<ItemTable> itemTableList = new List<ItemTable>();
    private List<VirusTable> virusTableList = new List<VirusTable>();

    private Sprite[] virusSpriteList;
    private Sprite[] itemSpriteList;
    private Sprite[] smallEnemySpriteList;
    private Sprite[] bigEnemySpriteList;
    private Sprite[] battleEnemySpriteList;

    private void Start()
    {
        unlockCount = 1;
        //ゲームテーブル初期化
        for (int enemyId = 1; enemyId <= 8; enemyId++)
        {
            gameTable.Add(enemyId, RESULT_KIND.YOU_LOSE);
        }

        masterDataRepository =
            Resources.Load<MasterDataRepository>("MasterData/MasterDataRepository");

        masterDataRepository.GetMstData_Enemy(out enemyTableList);
        masterDataRepository.GetMstData_Item(out itemTableList);
        masterDataRepository.GetMstData_Virus(out virusTableList);

        virusSpriteList = Resources.LoadAll<Sprite>("Images/Virus");
        itemSpriteList = Resources.LoadAll<Sprite>("Images/Items");
        smallEnemySpriteList = Resources.LoadAll<Sprite>("Images/Enemys/Small");
        bigEnemySpriteList = Resources.LoadAll<Sprite>("Images/Enemys/Big");
        battleEnemySpriteList = Resources.LoadAll<Sprite>("Images/Enemys/Battle");
    }

    /// <summary>
    /// ゲームシーンをロードする
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void UpdateGameTable(RESULT_KIND resultKind)
    {
        gameTable[battleId] = resultKind;
    }

    public RESULT_KIND CheckGameTable(int enemyId)
    {
        return gameTable[enemyId];
    }

    public RESULT_KIND CheckAllGameTable()
    {
        bool winFlg = gameTable.ContainsValue(RESULT_KIND.YOU_WIN);
        bool aliveFlg = gameTable.ContainsValue(RESULT_KIND.YOU_ALIVE);
        bool loseFlg = gameTable.ContainsValue(RESULT_KIND.YOU_LOSE);

        if (winFlg && !aliveFlg && !loseFlg)
        {
            return RESULT_KIND.YOU_WIN;
        }
        else if (!winFlg && aliveFlg && !loseFlg)
        {
            return RESULT_KIND.YOU_ALIVE;
        }
        else
        {
            return RESULT_KIND.YOU_LOSE;
        }
    }

    public Sprite GetVirusSprite(int virusId)
    {
        string virusName = virusTableList.Find(virus => virus.Id == virusId).AssetName;
        foreach (Sprite virusSprite in virusSpriteList)
        {
            if (virusName == virusSprite.name)
            {
                return virusSprite;
            }
        }

        Debug.LogError("スプライトが見つかりませんでした。" +
            "マスターデータの名前とResourcesの名前が一致しているか確認してください。");
        return virusSpriteList[0];
    }

    public Sprite GetItemSprite(int itemId)
    {
        string itemName = itemTableList.Find(item => item.Id == itemId).AssetName;
        foreach (Sprite itemSprite in itemSpriteList)
        {
            if (itemName == itemSprite.name)
            {
                return itemSprite;
            }
        }

        Debug.LogError("スプライトが見つかりませんでした。" +
            "マスターデータの名前とResourcesの名前が一致しているか確認してください。");
        return itemSpriteList[0];
    }

    /// <summary>
    /// 敵スプライト(小)リストを取得する
    /// </summary>
    /// <param name="smallSpriteList"></param>
    public Sprite GetSmallSprite(int enemyId)
    {
        string EnemyName = enemyTableList.Find(enemy => enemy.Id == enemyId).AssetName;
        foreach (Sprite smallEnemySprite in smallEnemySpriteList)
        {
            if (EnemyName == smallEnemySprite.name)
            {
                return smallEnemySprite;
            }
        }

        Debug.LogError("スプライトが見つかりませんでした。" +
            "マスターデータの名前とResourcesの名前が一致しているか確認してください。");
        return smallEnemySpriteList[0];
    }

    public Sprite GetBigSprite(int enemyId)
    {
        string EnemyName = enemyTableList.Find(enemy => enemy.Id == enemyId).AssetName;
        foreach (Sprite bigEnemySprite in bigEnemySpriteList)
        {
            if (EnemyName == bigEnemySprite.name)
            {
                return bigEnemySprite;
            }
        }

        Debug.LogError("スプライトが見つかりませんでした。" +
            "マスターデータの名前とResourcesの名前が一致しているか確認してください。");
        return bigEnemySpriteList[0];
    }

    public Sprite GetBattleSprite()
    {
        string EnemyName = enemyTableList.Find(enemy => enemy.Id == battleId).AssetName;
        foreach (Sprite battleEnemySprite in battleEnemySpriteList)
        {
            if (EnemyName == battleEnemySprite.name)
            {
                return battleEnemySprite;
            }
        }

        Debug.LogError("スプライトが見つかりませんでした。" +
            "マスターデータの名前とResourcesの名前が一致しているか確認してください。");
        return battleEnemySpriteList[0];
    }

    public void GetEnemyTable(out EnemyTable enemyTable)
    {
        enemyTable = enemyTableList.Find(table => table.Id == battleId);
    }

    public void GetEnemyTable(int enemyId, out EnemyTable enemyTable)
    {
        enemyTable = enemyTableList.Find(table => table.Id == enemyId);
    }

    public void GetItemTableList(out List<ItemTable> itemTableList)
    {
        itemTableList = this.itemTableList;
    }
}
