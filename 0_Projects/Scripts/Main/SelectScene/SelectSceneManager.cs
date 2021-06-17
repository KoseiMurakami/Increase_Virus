using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSceneManager : MonoBehaviour
{
    [SerializeField]
    private Transform canvas = default;

    [SerializeField]
    private Image enemyFrame_big = default;

    [SerializeField]
    private Button toBattleButton = default;

    [SerializeField]
    private GameObject toEndingButton = default;

    [SerializeField]
    private GameObject descriptionPanel = default;

    [SerializeField]
    private GameObject tutorialCanvas = default;

    private GameObject UIButtonPref;
    private GameObject ClearMarkerPref;
    private RectTransform canvasRect;

    void Start()
    {
        UIButtonPref = Resources.Load<GameObject>("Prefabs/UI/EnemyButton");
        ClearMarkerPref = Resources.Load<GameObject>("Prefabs/UI/CrearMarker");
        canvasRect = canvas.GetComponent<RectTransform>();

        SoundManager.Instance.PlayBgmByName("SelectBGM");

        //シーンイニシャライズ
        initSelectScene();

        //チュートリアル開始
        if (GameManager.Instance.battleId == 0)
        {
            StartCoroutine(startTutorial());
        }
    }

    /// <summary>
    /// セレクトシーン初期化
    /// </summary>
    /// <param name="alignCount"></param>
    private void initSelectScene()
    {
        int alignCount = 8;
        toBattleButton.interactable = false;
        descriptionPanel.SetActive(false);
        tutorialCanvas.SetActive(false);

        for (int i = 0; i < alignCount; i++)
        {
            var enemyId = i + 1;

            GameObject UIButton = Instantiate(UIButtonPref);
            RectTransform UIRect = UIButton.GetComponent<RectTransform>();

            UIButton.transform.SetParent(canvas);
            UIRect.localScale = new Vector2(1, 1);
            UIRect.anchoredPosition =
                new Vector2((canvasRect.sizeDelta.x + UIRect.sizeDelta.x) *
                (1 - alignCount + 2 * i) / (2 * (alignCount + 1)), 80);

            //unlock済みボタンの設定
            if (GameManager.Instance.unlockCount > i)
            {
                UIButton.GetComponent<Image>().sprite =
                    GameManager.Instance.GetSmallSprite(enemyId);
                UIButton.GetComponent<Button>().onClick.AddListener(() => selectEnemy(enemyId));
            }

            //CrearMarker設定
            GameObject clearMarker;
            switch (GameManager.Instance.CheckGameTable(enemyId))
            {
                case RESULT_KIND.YOU_WIN:
                    clearMarker = Instantiate(ClearMarkerPref);
                    clearMarker.transform.SetParent(UIButton.transform);
                    clearMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    clearMarker.GetComponent<RectTransform>().localScale = new Vector2(0.2f, 0.2f);
                    clearMarker.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ResultTexts/YouWin");
                    clearMarker.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                    break;
                case RESULT_KIND.YOU_ALIVE:
                    clearMarker = Instantiate(ClearMarkerPref);
                    clearMarker.transform.SetParent(UIButton.transform);
                    clearMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    clearMarker.GetComponent<RectTransform>().localScale = new Vector2(0.2f, 0.2f);
                    clearMarker.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/ResultTexts/YouAlive");
                    clearMarker.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                    break;
                case RESULT_KIND.YOU_LOSE:
                    break;
                default:
                    break;
            }
        }

        //エンディングボタンを表示
        if (GameManager.Instance.CheckAllGameTable() == RESULT_KIND.YOU_WIN ||
            GameManager.Instance.CheckAllGameTable() == RESULT_KIND.YOU_ALIVE)
        {
            toEndingButton.SetActive(true);
        }
    }

    /// <summary>
    /// 敵を選択したときの処理
    /// </summary>
    /// <param name="enemyId"></param>
    private void selectEnemy(int enemyId)
    {
        EnemyTable enemyTable = new EnemyTable();

        //画面中央にImage大を表示
        enemyFrame_big.sprite = GameManager.Instance.GetBigSprite(enemyId);

        //descriptionPanelを表示
        GameManager.Instance.GetEnemyTable(enemyId, out enemyTable);
        descriptionPanel.SetActive(true);
        descriptionPanel.transform.Find("enemyNameText").GetComponent<Text>().text = enemyTable.Name;
        descriptionPanel.transform.Find("enemyLevelText").GetComponent<Text>().text = "難易度 ";
        for (int i = 0; i < enemyTable.Level; i++)
        {
            descriptionPanel.transform.Find("enemyLevelText").GetComponent<Text>().text += "★";
        }
        for (int i = 0; i < 5 - enemyTable.Level; i++)
        {
            descriptionPanel.transform.Find("enemyLevelText").GetComponent<Text>().text += "☆";
        }
        descriptionPanel.transform.Find("enemyText").GetComponent<Text>().text = enemyTable.EnemyText;

        //battleIdの設定
        GameManager.Instance.battleId = enemyId;

        //ToBattoleボタン有効にする
        toBattleButton.interactable = true;

        SoundManager.Instance.PlaySeByName("SelectSE");
    }

    /// <summary>
    /// バトルシーンへ遷移
    /// </summary>
    public void goToBattle()
    {
        SoundManager.Instance.PlaySeByName("ToBattleSE");
        GameManager.Instance.LoadScene("BattleScene");
    }

    /// <summary>
    /// エンディングシーンへ遷移
    /// </summary>
    public void goToEnding()
    {
        GameManager.Instance.LoadScene("EndingScene");
    }

    /// <summary>
    /// セレクトシーン用チュートリアルを開始
    /// </summary>
    /// <returns></returns>
    private IEnumerator startTutorial()
    {
        GameObject tutoPanel = tutorialCanvas.transform.Find("TutorialPanel").gameObject;
        Text tutoText = tutoPanel.transform.Find("TutorialText").GetComponent<Text>();
        GameObject downArrow = tutorialCanvas.transform.Find("TutorialDownArrow").gameObject;
        GameObject rightArrow = tutorialCanvas.transform.Find("TutorialRightArrow").gameObject;
        GameObject clickGuardPanel = tutorialCanvas.transform.Find("ClickGuardPanel").gameObject;
        RectTransform downArrowRect = downArrow.GetComponent<RectTransform>();
        RectTransform rightArrowRect = rightArrow.GetComponent<RectTransform>();

        tutorialCanvas.SetActive(true);
        downArrow.SetActive(false);
        rightArrow.SetActive(false);

        float alpha;
        //テキストが浮き上がる(1秒)
        for (int i = 0; i < 4; i++)
        {
            alpha = 0;
            tutoText.color = new Color(255, 255, 255, alpha);

            switch (i)
            {
                case 0:
                    tutoText.text = "ウイルスの世界へようこそ。";
                    break;
                case 1:
                    tutoText.text = "このゲームでは、あなたはウイルスたちの指揮官となり人間と戦います。";
                    break;
                case 2:
                    tutoText.text = "ウイルスたちの「繁栄」を目指しましょう。";
                    break;
                case 3:
                    tutoText.text = "それでは、さっそく人間と戦ってみましょう。";
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

        tutoPanel.SetActive(false);
        clickGuardPanel.SetActive(false);

        //敵選択完了まで下矢印上下
        downArrow.SetActive(true);
        float tmpPos = 0;
        while (GameManager.Instance.battleId == 0)
        {
            downArrowRect.anchoredPosition += new Vector2(0, Mathf.Sin(4 * tmpPos * Mathf.Deg2Rad) / 4);
            tmpPos++;
            yield return null;
        }

        downArrow.SetActive(false);

        //右矢印往復
        rightArrow.SetActive(true);
        tmpPos = 0;
        while (true)
        {
            rightArrowRect.anchoredPosition -= new Vector2(Mathf.Sin(4 * tmpPos * Mathf.Deg2Rad) / 4, 0);
            tmpPos++;
            yield return null;
        }
    }
}
