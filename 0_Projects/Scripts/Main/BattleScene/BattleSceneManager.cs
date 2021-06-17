using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField]
    private Text timeText = default;

    [SerializeField]
    private Text moneyText = default;

    [SerializeField]
    private Slider HpBar = default;

    [SerializeField]
    private SpriteRenderer enemyImage = default;

    [SerializeField]
    private Transform itemBoxPanel = default;

    [SerializeField]
    private GameObject frontCanvas = default;

    [SerializeField]
    private GameObject tutorialCanvas = default;

    private bool isActiveFlg = false;
    private int money = 0;
    private float increaseInterval = 0.5f;
    private float Hp;
    private int innerVirusId = 0;

    private GameObject virusPref;
    private GameObject medicinePref;
    private EnemyTable enemyTable = new EnemyTable();
    private List<ItemTable> itemTableList = new List<ItemTable>();
    private List<MyVirusMove> myVirusMoveList = new List<MyVirusMove>();
    private GameObject countObj;
    private GameObject descriptionPanel;
    private Text itemNameText;
    private Text itemPriceText;
    private Text itemEffectText;


    void Start()
    {
        virusPref = Resources.Load<GameObject>("Prefabs/Virus/Virus");
        medicinePref = Resources.Load<GameObject>("Prefabs/Items/Medicine");
        GameManager.Instance.GetEnemyTable(out enemyTable);
        GameManager.Instance.GetItemTableList(out itemTableList);
        countObj = frontCanvas.transform.Find("CountImage").gameObject;
        descriptionPanel = frontCanvas.transform.Find("DescriptionPanel").gameObject;
        itemNameText = descriptionPanel.transform.Find("ItemNameText").GetComponent<Text>();
        itemPriceText = descriptionPanel.transform.Find("ItemPriceText").GetComponent<Text>();
        itemEffectText = descriptionPanel.transform.Find("ItemEffectText").GetComponent<Text>();

        SoundManager.Instance.PlayBgmByName("BattleBGM");

        initBattleScene();

        if (GameManager.Instance.battleId == 1)
        {
            StartCoroutine(startTutorial());
        }
        else
        {
            StartCoroutine(battleStart());
        }
    }

    /// <summary>
    /// バトルシーン初期化
    /// </summary>
    private void initBattleScene()
    {
        this.Hp = enemyTable.Hp;
        int minute = enemyTable.Time;
        timeText.text = minute.ToString("00") + " : 00";
        tutorialCanvas.SetActive(false);
        frontCanvas.SetActive(false);
        countObj.SetActive(false);
        descriptionPanel.SetActive(false);

        int itemIndex = 1;
        foreach (Transform itemBox in itemBoxPanel)
        {
            DragObj dragObj;

            itemBox.GetComponent<Image>().sprite =
                GameManager.Instance.GetItemSprite(itemIndex);
            dragObj = itemBox.gameObject.AddComponent<DragObj>();
            dragObj.ItemId = itemIndex;

            itemIndex++;
        }

        enemyImage.sprite = GameManager.Instance.GetBattleSprite();
    }

    /// <summary>
    /// ウイルスを体内に送り込む
    /// </summary>
    /// <param name="virusId"></param>
    public void ThrowVirusIntoInside(int virusId)
    {
        if (!isActiveFlg)
        {
            return;
        }
        GameObject virusObj = Instantiate(virusPref);
        MyVirusMove virusMove = virusObj.GetComponent<MyVirusMove>();
        virusMove.VirusId = innerVirusId;
        virusObj.GetComponent<SpriteRenderer>().sprite =
            GameManager.Instance.GetVirusSprite(virusId);
        virusObj.transform.position = new Vector2(Random.Range(0.0f, 6.5f), 1.8f);
        myVirusMoveList.Add(virusMove);
        SoundManager.Instance.PlaySeByName("AppearSE");
        innerVirusId++;
    }

    /// <summary>
    /// 体内ウイルスデータを削除する
    /// </summary>
    /// <param name="virusId"></param>
    public void RemoveVirusMoveList(int virusId)
    {
        myVirusMoveList.Remove(myVirusMoveList.Find(virusMove => virusMove.VirusId == virusId));
    }

    /// <summary>
    /// 投薬レベルを計算する
    /// </summary>
    /// <param name="medicineLevel"></param>
    /// <param name="leftMinute"></param>
    /// <param name="throwInterval"></param>
    /// <param name="throwCount"></param>
    private void calcThrowMedicineLevel(int medicineLevel, int leftMinute, ref float throwInterval, ref int throwCount)
    {
        if (!isActiveFlg)
        {
            return;
        }

        switch (medicineLevel)
        {
            case 0:
                throwInterval = 5.0f;
                throwCount = 0;
                break;
            case 1:
                throwInterval = 60.0f / (enemyTable.Time + 1 - leftMinute);
                throwCount = 3;
                break;
            case 2:
                throwInterval = 60.0f / ((enemyTable.Time + 1 - leftMinute) * (enemyTable.Time + 1 - leftMinute));
                throwCount = 5;
                break;
        }
    }

    /// <summary>
    /// アイテムを使用する
    /// </summary>
    /// <param name="itemId"></param>
    public void UseItem(int itemId)
    {
        if (!isActiveFlg)
        {
            return;
        }

        //使おうとするアイテムの値段より所持金が少ないならリターン
        int price = itemTableList.Find(itemTable => itemTable.Id == itemId).Price;
        if (money < price)
        {
            return;
        }

        switch (itemId)
        {
            case 1:
                //資金増加間隔が0.1秒減る
                if (increaseInterval <= 0.11f)
                {
                    return;
                }
                money -= price;
                SoundManager.Instance.PlaySeByName("HurikakeSE");
                increaseInterval -= 0.1f;
                break;
            case 2:
                //ウイルスを1体投入する
                money -= price;
                ThrowVirusIntoInside(1);
                break;
            case 3:
                //体内にいるウイルスの攻撃力を2倍にする
                money -= price;
                SoundManager.Instance.PlaySeByName("PowerUpSE");
                foreach (MyVirusMove virusClass in myVirusMoveList)
                {
                    virusClass.Damage *= 2;
                }
                break;
            case 4:
                //体内にいるウイルスの数を2倍にする
                money -= price;
                int throwVirusCount = myVirusMoveList.Count;
                for (int i = 0; i < throwVirusCount; i++)
                {
                    ThrowVirusIntoInside(1);
                }
                break;
            case 5:
                //ウイルス10体投入する
                money -= price;
                for (int i = 0; i < 10; i++)
                {
                    ThrowVirusIntoInside(1);
                }
                break;
            case 6:
                //ウイルス50体投入する
                money -= price;
                for (int i = 0; i < 50; i++)
                {
                    ThrowVirusIntoInside(1);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="value"></param>
    public void Damage(float value)
    {
        if (!isActiveFlg)
        {
            return;
        }

        Hp -= value;
        HpBar.value = this.Hp / enemyTable.Hp;
        SoundManager.Instance.PlaySeByName("DamageSE");

        if (Hp <= 0)
        {
            //全コルーチン停止 + YouWin演出
            StopAllCoroutines();
            StartCoroutine(directResult(RESULT_KIND.YOU_WIN));
        }
    }

    /// <summary>
    /// アイテム説明用ディスプレイを表示する
    /// </summary>
    /// <param name="itemId"></param>
    public void DisplayDescription(int itemId)
    {
        descriptionPanel.SetActive(true);
        itemNameText.text = itemTableList.Find(itemTable => itemTable.Id == itemId).Name;
        itemPriceText.text = "必要資金 " + itemTableList.Find(itemTable => itemTable.Id == itemId).Price;
        itemEffectText.text = itemTableList.Find(itemTable => itemTable.Id == itemId).Text;
    }

    /// <summary>
    /// アイテム説明用ディスプレイを非表示にする
    /// </summary>
    public void HiddenDescription()
    {
        descriptionPanel.SetActive(false);
    }

    /// <summary>
    /// タイマーを走らせる
    /// </summary>
    /// <param name="minute"></param>
    /// <param name="seconds"></param>
    /// <param name="oldSeconds"></param>
    /// <returns></returns>
    private bool runTimer(ref int minute, ref float seconds, ref float oldSeconds)
    {
        /*タイマー計算*/
        if (seconds <= 0f)
        {
            minute--;
            seconds += 60;
        }

        seconds -= Time.deltaTime;

        //残り時間が少なくなった時の演出
        if (enemyTable.Time >= 2 &&
            minute == 0 &&
            (int)seconds == 59 &&
            (int)oldSeconds == 0)
        {
            SoundManager.Instance.SetBGMPitchUP();
        }

        if ((int)seconds != (int)oldSeconds)
        {
            timeText.text = minute.ToString("00") + " : " + ((int)seconds).ToString("00");
            if (enemyTable.Time >= 2 && minute == 0)
            {
                timeText.color = new Color(255,0,0);
            }
        }

        oldSeconds = seconds;

        if (seconds <= 0f && minute <= 0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// セレクトシーンに戻る
    /// </summary>
    public void PushBackButton()
    {
        if (!isActiveFlg)
        {
            return;
        }

        if (myVirusMoveList.Count > 0)
        {
            GameManager.Instance.UpdateGameTable(RESULT_KIND.YOU_ALIVE);
            if (GameManager.Instance.battleId == GameManager.Instance.unlockCount &&
                    GameManager.Instance.unlockCount < 8)
            {

                GameManager.Instance.unlockCount++;
            }
        }
        SoundManager.Instance.PlaySeByName("BackButtonSE");
        GameManager.Instance.LoadScene("SelectScene");
    }

    /// <summary>
    /// バトルをスタートさせる
    /// </summary>
    /// <returns></returns>
    private IEnumerator battleStart()
    {
        /*タイマー用*/
        int minute = enemyTable.Time;
        float seconds = 0;
        float oldSeconds = 0;

        /*投薬用*/
        float throwElapsed = 0;
        float throwInterval = 0;
        int throwCount = 0;

        Image countImage = countObj.GetComponent<Image>();
        Sprite[] countSprite = Resources.LoadAll<Sprite>("Images/Counts");

        frontCanvas.SetActive(true);
        countObj.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            countImage.sprite = countSprite[2 - i];
            yield return new WaitForSeconds(1);
        }

        Destroy(countObj);

        isActiveFlg = true;
        StartCoroutine("increaseMoney");

        while (runTimer(ref minute, ref seconds, ref oldSeconds))
        {
            throwElapsed += Time.deltaTime;
            calcThrowMedicineLevel(enemyTable.MedicineLevel, minute, ref throwInterval, ref throwCount);

            //一定間隔で投薬を行う
            if (throwElapsed >= throwInterval)
            {
                for (int i = 0; i < throwCount; i++)
                {
                    GameObject medicineObj = Instantiate(medicinePref);
                }

                throwElapsed = 0.0f;
            }
            yield return null;
        }

        StopCoroutine("increaseMoney");

        //最後のウイルスが1秒以上生き残ったらAliveとする。
        if (myVirusMoveList.Count > 0)
        {
            isActiveFlg = false;
            yield return new WaitForSeconds(1);
        }

        //ウイルスが残っていればYouAlive演出
        if (myVirusMoveList.Count > 0)
        {
            StartCoroutine(directResult(RESULT_KIND.YOU_ALIVE));
        }
        //ウイルスが残っていなければYouLose演出
        else
        {
            StartCoroutine(directResult(RESULT_KIND.YOU_LOSE));
        }
    }

    /// <summary>
    /// 資金を一定間隔で増やす
    /// </summary>
    /// <returns></returns>
    private IEnumerator increaseMoney()
    {
        while (true)
        {
            money += 100;
            moneyText.text = money.ToString("D8");

            yield return new WaitForSeconds(increaseInterval);
        }
    }

    /// <summary>
    /// リザルト演出を行う
    /// </summary>
    /// <param name="resultKind"></param>
    /// <returns></returns>
    private IEnumerator directResult(RESULT_KIND resultKind)
    {
        GameObject resultObj = frontCanvas.transform.Find("ResultImage").gameObject;
        RectTransform resultRect = resultObj.GetComponent<RectTransform>();
        Image resultImage = resultObj.GetComponent<Image>();

        isActiveFlg = false;
        SoundManager.Instance.StopBgm();

        switch (resultKind)
        {
            case RESULT_KIND.YOU_WIN:
                resultImage.sprite = Resources.Load<Sprite>("Images/ResultTexts/YouWin");
                GameManager.Instance.UpdateGameTable(resultKind);
                SoundManager.Instance.PlaySeByName("YouWinSE");
                if (GameManager.Instance.battleId == GameManager.Instance.unlockCount &&
                    GameManager.Instance.unlockCount < 8)
                {
 
                    GameManager.Instance.unlockCount++;
                }
                break;
            case RESULT_KIND.YOU_ALIVE:
                resultImage.sprite = Resources.Load<Sprite>("Images/ResultTexts/YouAlive");
                GameManager.Instance.UpdateGameTable(resultKind);
                SoundManager.Instance.PlaySeByName("YouWinSE");
                if (GameManager.Instance.battleId == GameManager.Instance.unlockCount &&
                    GameManager.Instance.unlockCount < 8)
                {

                    GameManager.Instance.unlockCount++;
                }
                break;
            case RESULT_KIND.YOU_LOSE:
                resultImage.sprite = Resources.Load<Sprite>("Images/ResultTexts/YouLose");
                SoundManager.Instance.PlaySeByName("YouLoseSE");
                break;
            default:
                break;
        }

        while (resultRect.position.y >= 0)
        {
            resultRect.position -= new Vector3(0, 2 * Time.deltaTime, 0);
            yield return null;
        }

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.LoadScene("SelectScene");
            }
            yield return null;
        }
    }

    /// <summary>
    /// バトルシーン用チュートリアルを開始
    /// </summary>
    /// <returns></returns>
    private IEnumerator startTutorial()
    {
        float alpha;
        GameObject tutoPanel = tutorialCanvas.transform.Find("TutorialPanel").gameObject;
        Text tutoText = tutoPanel.transform.Find("TutorialText").GetComponent<Text>();
        GameObject tutoItem = tutorialCanvas.transform.Find("TutorialItem").gameObject;
        GameObject clickGuardPanel = tutorialCanvas.transform.Find("ClickGuardPanel").gameObject;
        RectTransform tutoItemRect = tutoItem.GetComponent<RectTransform>();

        tutorialCanvas.SetActive(true);
        tutoItem.SetActive(false);
        money = 1000;
        moneyText.text = money.ToString("D8");

        //テキストが浮き上がる(1秒)
        for (int i = 0; i < 4; i++)
        {
            alpha = 0;
            tutoText.color = new Color(255, 255, 255, alpha);

            switch (i)
            {
                case 0:
                    tutoText.text = "指揮官といってもあなたの仕事はウイルスを人間の体内に送り込んだり、";
                    break;
                case 1:
                    tutoText.text = "支援物資を体内に供給するだけです。";
                    break;
                case 2:
                    tutoText.text = "左に見えているアイテムを敵の上にドラッグしてみましょう。";
                    break;
                case 3:
                    tutoText.text = "カーソルをアイテムに合わせると効果が確認できます。";
                    break;
            }

            yield return new WaitForSeconds(0.5f);

            while (alpha < 1)
            {
                //画面クリックするとすぐに浮き上がる
                if (Input.GetMouseButtonDown(0))
                {
                    alpha = 1;
                }

                tutoText.color = new Color(255, 255, 255, alpha);
                alpha += Time.deltaTime;
                yield return null;
            }

            //浮き上がった後、クリックすると次のテキストへ(繰り返し)
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            SoundManager.Instance.PlaySeByName("ClickSE");
        }

        frontCanvas.SetActive(true);
        tutoPanel.SetActive(false);
        clickGuardPanel.SetActive(false);

        //ダミーウイルス付き食パン左右往復
        tutoItem.SetActive(true);
        isActiveFlg = true;
        float tmpCount = 0;
        Vector2 tempVec = tutoItemRect.anchoredPosition;
        while (myVirusMoveList.Count == 0)
        {
            if (tmpCount > 100)
            {
                tmpCount = 0;
            }

            tutoItemRect.anchoredPosition = tempVec + new Vector2(tmpCount, 0);
            tmpCount++;
            yield return null;
        }
        moneyText.text = money.ToString("D8");
        isActiveFlg = false;

        tutoItem.SetActive(false);
        tutoPanel.SetActive(true);
        clickGuardPanel.SetActive(true);

        //テキストが浮き上がる(1秒)
        for (int i = 0; i < 4; i++)
        {
            alpha = 0;
            tutoText.color = new Color(255, 255, 255, alpha);

            switch (i)
            {
                case 0:
                    tutoText.text = "OKです！";
                    break;
                case 1:
                    tutoText.text = "すると、敵の体内モニターに今送り込んだウイルスが映し出されます。";
                    break;
                case 2:
                    tutoText.text = "そして、体内のウイルスは自動的に敵を攻撃します。";
                    break;
                case 3:
                    tutoText.text = "指揮官としてうまくウイルス達を導いてください！";
                    break;
            }

            yield return new WaitForSeconds(0.5f);

            while (alpha < 1)
            {
                //画面クリックするとすぐに浮き上がる
                if (Input.GetMouseButtonDown(0))
                {
                    alpha = 1;
                }

                tutoText.color = new Color(255, 255, 255, alpha);
                alpha += Time.deltaTime;
                yield return null;
            }

            //浮き上がった後、クリックすると次のテキストへ(繰り返し)
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            SoundManager.Instance.PlaySeByName("ClickSE");
        }

        tutorialCanvas.SetActive(false);

        StartCoroutine(battleStart());

        yield break;
    }
}
