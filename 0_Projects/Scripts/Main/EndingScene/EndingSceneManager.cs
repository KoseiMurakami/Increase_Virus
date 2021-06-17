using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject endingTextObj = default;

    [SerializeField]
    private Image endMessImage = default;

    void Start()
    {
        endMessImage.color = new Color(255, 255, 255, 0);

        SoundManager.Instance.StopBgm();
        //エンディングAを流す
        if (GameManager.Instance.CheckAllGameTable() == RESULT_KIND.YOU_WIN)
        {
            StartCoroutine(endingA());
        }
        //エンディングBを流す
        else
        {
            StartCoroutine(endingB());
        }
    }

    private IEnumerator endingA()
    {
        Text endingText = endingTextObj.GetComponent<Text>();

        float alpha;

        //テキストが浮き上がる(1秒)
        for (int i = 0; i < 4; i++)
        {
            alpha = 0;
            endingText.color = new Color(255, 255, 255, alpha);

            switch (i)
            {
                case 0:
                    endingText.text = "すべての人間を討伐しました。";
                    break;
                case 1:
                    endingText.text = "ウイルス軍の勝利です。";
                    break;
                case 2:
                    endingText.text = "しかし、人間を全滅させてしまったおかげで自らの生活環境を失ってしまいました。";
                    break;
                case 3:
                    endingText.text = "ほどなくして、生活できなくなったウイルス軍も全滅してしまいました。";
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

                endingText.color = new Color(255, 255, 255, alpha);
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

        endingText.text = "";

        //バッドエンド浮き上がる
        SoundManager.Instance.PlaySeByName("BadEndSE");
        endMessImage.sprite = Resources.Load<Sprite>("Images/ResultTexts/BadEnd");
        endMessImage.SetNativeSize();
        alpha = 0;
        while (alpha < 1)
        {
            endMessImage.color = new Color(255, 255, 255, alpha);
            alpha += Time.deltaTime;
            yield return null;
        }

        //最後のテキストループ終了後、クリックするとタイトルシーンへ
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.LoadScene("TitleScene");
            }
            yield return null;
        }
    }

    private IEnumerator endingB()
    {
        Text endingText = endingTextObj.GetComponent<Text>();

        float alpha;
        //テキストが浮き上がる(1秒)
        for (int i = 0; i < 4; i++)
        {
            alpha = 0;
            endingText.color = new Color(255, 255, 255, alpha);

            switch (i)
            {
                case 0:
                    endingText.text = "ウイルスたちは人間を殺しませんでした。";
                    break;
                case 1:
                    endingText.text = "人間は抗体を作ったり薬を使ったりしてウイルスに抵抗してくるので、仲間のウイルスがやられることもあります。";
                    break;
                case 2:
                    endingText.text = "しかし、それでも種として生き残ることのほうがウイルスたちにとって重要なのです。";
                    break;
                case 3:
                    endingText.text = "そうやって人間と共存することで、ウイルスたちは繁栄することができました。";
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

                endingText.color = new Color(255, 255, 255, alpha);
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

        endingText.text = "";

        //トゥルーエンド浮き上がる
        SoundManager.Instance.PlaySeByName("GameClearSE");
        endMessImage.sprite = Resources.Load<Sprite>("Images/ResultTexts/GameClear");
        endMessImage.SetNativeSize();
        alpha = 0;
        while (alpha < 1)
        {
            endMessImage.color = new Color(255, 255, 255, alpha);
            alpha += Time.deltaTime;
            yield return null;
        }

        //最後のテキストループ終了後、クリックするとタイトルシーンへ
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.LoadScene("TitleScene");
            }
            yield return null;
        }
    }
}
