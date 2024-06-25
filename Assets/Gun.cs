using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject muzzle;
    public GameObject bulletPrefab;
    public BulletData load;
    [Range(0.0f, 90.0f)]
    public float angle;
    float time;
    public bool fired = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        float angler = Mathf.SmoothStep(0.0f, angle, time);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angler);
        time += Time.deltaTime * 1.0f;

        if(angler == angle && !fired && load != null) {
            UIMaster.instance.SetFireButtonActive();
        }
    }

    public void Fire() {
        muzzle.GetComponent<AudioSource>().Play();
        var obj = Instantiate(bulletPrefab);
        obj.transform.position = muzzle.transform.position;
        CameraViewer.instance.attachedObject = obj;

        BallisticObject ball = obj.GetComponent<BallisticObject>();
        ball.bulletData = load;
        ball.trajectory = UIMaster.instance.trajectory;
        ball.Launch(angle);
        fired = true;

        UIMaster.instance.ballisticObject = ball;
    }
}
