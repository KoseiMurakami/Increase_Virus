using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineMove : MonoBehaviour
{
    void Start()
    {
        transform.position = new Vector2(Random.Range(0.0f, 6.5f), 1.8f);
        float force = Random.Range(0.5f, 3.0f);
        float theta = Random.Range(180, 360) * Mathf.Deg2Rad;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(force * Mathf.Cos(theta), force * Mathf.Sin(theta)), ForceMode2D.Impulse);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Virus"))
        {
            Destroy(gameObject);
        }
    }
}
