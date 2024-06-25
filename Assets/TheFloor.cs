using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Hit!");
        Debug.Log(collision.gameObject.transform.position);
        Rigidbody2D rb = collision.rigidbody;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        rb.gameObject.SetActive(false);
        UIMaster.instance.fieldToggled(true);

        UIMaster.instance.hitPanel.SetActive(true);
        UIMaster.instance.hitText.text = $"Hit at {(collision.gameObject.transform.position.x / 1000).ToString("F2")}km!";
    }
}
