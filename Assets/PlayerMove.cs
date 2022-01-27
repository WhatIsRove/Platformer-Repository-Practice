using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;

    [SerializeField] public Rigidbody2D rb;

    float horizontalMove;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(horizontalMove, 0, 0) * moveSpeed * Time.fixedDeltaTime;
    }
}
