using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private Image image1 = default;

    [SerializeField]
    private Image image2 = default;

    private void Start()
    {
        SoundManager.Instance.PlayBgmByName("TitleBGM");
        StartCoroutine("startLogoAnimation");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.Instance.LoadScene("SelectScene");
        }
    }

    public void ChangeAudio()
    {
        if (SoundManager.Instance.AudioLevel == 3)
        {
            SoundManager.Instance.AudioLevel = 0;
        }

        SoundManager.Instance.AudioLevel++;
    }

    private IEnumerator startLogoAnimation()
    {
        float alpha = 0;
        while (alpha < 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                alpha = 1;
            }
            image1.color = new Color(255,255,255,alpha);
            image2.color = new Color(255,255,255,alpha);

            alpha += Time.deltaTime / 3.0f;
            yield return null;
        }

        StartCoroutine("flashingLogoAnimation");

        yield break;
    }

    private IEnumerator flashingLogoAnimation()
    {
        float alpha = 0;
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.Instance.LoadScene("SelectScene");
            }
            image2.color = new Color(255, 255, 255, (Mathf.Cos(2 * alpha) + 1) / 2);

            alpha += Time.deltaTime;
            yield return null;
        }
    }
}
