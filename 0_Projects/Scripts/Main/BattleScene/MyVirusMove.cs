using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVirusMove : MonoBehaviour
{
    private BattleSceneManager battleSceneManager;
    private Rigidbody2D rb;

    public int VirusId { set; get; }
    public int Damage { set; get; }

    void Start()
    {
        battleSceneManager = FindObjectOfType<BattleSceneManager>();
        rb = GetComponent<Rigidbody2D>();

        Damage = 1;

        StartCoroutine("virusMove");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Medicine"))
        {
            battleSceneManager.RemoveVirusMoveList(VirusId);
            SoundManager.Instance.PlaySeByName("DestroySE");
            Destroy(gameObject);
        }
    }

    private IEnumerator virusMove()
    {
        int beforeDamage = Damage;

        /*タイマー用*/
        float jumpTimeOut = Random.Range(1, 5);
        float damageTimeOut = 5.0f;
        float jumpTimeElapsed = 0.0f;
        float damageTimeElapsed = 0.0f;

        while (true)
        {
            jumpTimeElapsed += Time.deltaTime;
            damageTimeElapsed += Time.deltaTime;

            if (Damage != beforeDamage)
            {
                float scale = 1 * Mathf.Pow(1.1f, Damage);
                transform.localScale = new Vector3(scale, scale, scale);
            }

            //1～5秒の間隔で飛び跳ねる
            if (jumpTimeElapsed >= jumpTimeOut)
            {
                float force = Random.Range(0.5f,3.0f);
                float theta = Random.Range(1, 180) * Mathf.Deg2Rad;
                rb.AddForce(new Vector2(force * Mathf.Cos(theta), force * Mathf.Sin(theta)), ForceMode2D.Impulse);
                jumpTimeElapsed = 0.0f;
                jumpTimeOut = Random.Range(1, 5);
                SoundManager.Instance.PlaySeByName("JumpSE");
            }
            if (damageTimeElapsed >= damageTimeOut)
            {
                battleSceneManager.Damage(Damage);
                damageTimeElapsed -= damageTimeOut;
            }

            if (transform.position.y < -5.0f)
            {
                Destroy(gameObject);
            }

            beforeDamage = Damage;

            yield return new WaitForFixedUpdate();
        }
    }
}
