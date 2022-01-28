using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damage = 1; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}
