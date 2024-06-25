using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewer : MonoBehaviour
{
    public GameObject attachedObject;
    public Camera cameraComponent;

    public static CameraViewer instance;

    public bool field;

    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!field) {
            transform.position = new Vector3(attachedObject.transform.position.x, attachedObject.transform.position.y, transform.position.z);
        } else {
            transform.position = new Vector3(3000.0f, 1000.0f, -10.0f);
        }
    }

    public void SetField(bool field) {
        this.field = field;
        if(UIMaster.instance.gun.fired) {
            cameraComponent.orthographicSize = field ? 2500f : 0.2f;
        } else {
            cameraComponent.orthographicSize = field ? 2500f : 2f;
        }
    }

    public void UpdateField() {
        cameraComponent.orthographicSize = field ? 2500f : 0.2f;
    }
}
