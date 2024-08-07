using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;
    private float halfWidth;

    private GameManager gameManager;

    public Sprite deadPlayerSprite;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Calculate half the width of the player object
        if (TryGetComponent<SpriteRenderer>(out var renderer))
             halfWidth = renderer.bounds.extents.x * 0.9f;
        else
            Debug.LogError("SpriteRenderer component missing on player object.");

        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {       
        if (Input.GetMouseButton(0) && !gameManager.IsPaused()) 
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchPos.z = 0f; // Ensure z is zero since we're working in 2D

            if (Mathf.Abs(touchPos.x - transform.position.x) > halfWidth)
            {
                if (touchPos.x < transform.position.x - halfWidth)
                    rb.AddForce(Vector2.left * moveSpeed * Time.deltaTime);
                else
                    rb.AddForce(Vector2.right * moveSpeed * Time.deltaTime); 
            }
            else
                rb.velocity = Vector2.zero;

        }
        else
            rb.velocity = Vector2.zero;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            ChangeToDeadSprite();
            StartCoroutine(DelayedGameOver());
        }
    }
    
    private IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 2 seconds
        gameManager.GameOver();
    }

    private void ChangeToDeadSprite()
    {
        if (TryGetComponent<SpriteRenderer>(out var renderer))
            renderer.sprite = deadPlayerSprite;
    }
}
