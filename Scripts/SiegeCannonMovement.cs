using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class SiegeCannonMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;
    private bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;

        animator.SetFloat("moveX", Mathf.Abs(moveX));
        animator.SetFloat("moveY", moveY);

        if (moveX > 0 && !facingRight || moveX < 0 && facingRight)
        {
            flip();
        }
    }

    void flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
