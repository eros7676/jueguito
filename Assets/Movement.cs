using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    public InputActionReference move;
    public float speed;

    public float k = 10f;
    public float distanceGrab = 3f;

    private Vector2 moveDirection;
    private Vector2 mousePosition;
    private bool mousePressing;
    private Rigidbody2D rb2;
    private Transform containerTransform;
    private Collider2D triangleCollider;

    private bool isGrabbing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("This is a test!");
        rb2 = GetComponent<Rigidbody2D>();
        containerTransform = transform.GetChild(0);
        triangleCollider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePressing = Input.GetMouseButton(0);
    }

    void FixedUpdate()
    {
        // move
        rb2.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);

        // get mouse position & normalize
        Vector2 dir = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);        

        dir = normalizeVector(dir, k);

        // rotate container
        containerTransform.LookAt(transform.position + (Vector3) dir);

        // raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        Debug.DrawRay(transform.position, dir, Color.red);

        // apply force to hitted object
        Rigidbody2D rbhit;

        if (!mousePressing) isGrabbing = false;

        if (hit && hit.transform.TryGetComponent<Rigidbody2D>(out rbhit) && mousePressing)
        {

            if (hit.distance >= distanceGrab)
            {
                rbhit.AddForce(-dir);
            }
            else if (!isGrabbing)
            {
                // rbhit.gravityScale = 0;
                rbhit.bodyType = RigidbodyType2D.Kinematic;
                hit.transform.SetParent(transform.GetChild(0).GetChild(0));
                hit.transform.localPosition = Vector3.zero;

                isGrabbing = true;
            }

            
        }
        

    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if(!collision.CompareTag("Movable")) return;

    //     flagGrabbing = true;
    // }

    // void OnTriggerExit2D(Collider2D collision)
    // {
    //     if(!collision.CompareTag("Movable")) return;

    //     flagGrabbing = false;
    // }

    static Vector2 normalizeVector(Vector2 v, float k)
    {
        float aux = (float) (k / Math.Sqrt(Math.Pow(v.x, 2) + Math.Pow(v.y, 2)));

        return new Vector2(v.x * aux, v.y * aux);
    }
}
