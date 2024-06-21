using System;
using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

public class BallisticObject : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public Vector2 initialVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.velocity = initialVelocity;
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector2 drag = ForceDrag(rigidBody.velocity);
        Debug.DrawRay(rigidBody.position, -drag);
        Debug.DrawRay(rigidBody.position, rigidBody.velocity, Color.red);
        // Apply drag
        rigidBody.velocity = rigidBody.velocity - drag * Time.fixedDeltaTime / rigidBody.mass;
    }

    Vector2 ForceDrag(Vector2 velocity) {
        // Calculate drag
        //     ne   1/2    p (air)  u squared                                                           c        time force applied
        return 1.293f * (velocity.normalized * velocity.sqrMagnitude) * 0.573f * Mathf.Pow(0.00822f, 2);
    }

    float AirDensity(Vector2 position) {
        throw new NotImplementedException();
    }

    float MachNumber(float speed) {
        return speed / 343f;
    }

    float CrossSectionalArea() {
        throw new NotImplementedException();
    }
}
