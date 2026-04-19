using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    public InputActionReference move;
    public float speed;

    public float distanceGrab = 3f;
    public float gVel = 30f;

    private Vector2 moveDirection;
    private Vector2 mousePosition;
    private bool LMousePressing, RMousePressing, ignoreRMB;
    private Rigidbody2D rb2;
    private Transform container, grabbingBox, gPoint;
    private Rigidbody2D grabbingBoxRB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        container = transform.GetChild(0);
        gPoint = transform.GetChild(0).GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        LMousePressing = Input.GetMouseButton(0);
        RMousePressing = Input.GetMouseButton(1);

        if (!RMousePressing) ignoreRMB = false;
    }

    void FixedUpdate()
    {
        // move
        rb2.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);

        // get mouse position & normalize
        Vector3 dir= mousePosition - (Vector2)transform.position;
        Vector3 dirN = dir.normalized;        

        // rotate container
        container.LookAt(transform.position + dirN);

        // raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirN);
        Debug.DrawRay(transform.position, dirN * dir.magnitude, Color.red);
        Rigidbody2D rbhit;


        




        // Apply Force to Box
        if (grabbingBox != null)
        {
            Vector2 delta = gPoint.position - grabbingBox.position;

            grabbingBoxRB.linearVelocity = delta.normalized * delta.magnitude / 0.1f;

            if (LMousePressing)
            {
                // grabbingBoxRB.AddForce(dirN * gVel * 100);
                grabbingBoxRB.linearVelocity = dirN * gVel;
                
                grabbingBox = null;
                ignoreRMB = true;
            }
        }


        // Stop Grabbing
        if (!RMousePressing) grabbingBox = null;

        else if (!ignoreRMB && grabbingBox == null && hit && hit.transform.TryGetComponent<Rigidbody2D>(out rbhit))
        {

            if (hit.distance >= distanceGrab)
            {
                rbhit.AddForce(-dirN * gVel);
            }
            else
            {
                grabbingBox = hit.transform;
                grabbingBoxRB = rbhit;
            }
        }
        

    }
}
