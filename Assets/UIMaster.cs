using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMaster : MonoBehaviour
{
    public TextMeshProUGUI timescale;
    public GameObject initialPanel;
    public Button button;

    public TextMeshProUGUI info;

    public TextMeshProUGUI launchAngle;

    public static UIMaster instance;

    public Gun gun;

    public Toggle bulletToggle;
    public Toggle fieldToggle;

    public BallisticObject ballisticObject;

    public Vector2 windVelocity;
    public TextMeshProUGUI windVelocityText;

    public TextMeshProUGUI gravityText;

    public LineRenderer trajectory;

    public BulletData[] cartridges;

    public TextMeshProUGUI cartridgeInfoText;

    public TextMeshProUGUI hitText;

    public GameObject hitPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ballisticObject != null) {
            info.text = $"Velocity: {ballisticObject.rigidBody.velocity.magnitude.ToString("F1")}m/s [{(Mathf.Rad2Deg * Mathf.Atan(ballisticObject.rigidBody.velocity.y/ballisticObject.rigidBody.velocity.x)).ToString("F1")}]\nMach: M{Mathf.Floor(ballisticObject.machNumber)}\nAltitude: {ballisticObject.rigidBody.position.y}m\nForce Drag: {ballisticObject.drag.magnitude.ToString("E2")}N\nFnet: {(ballisticObject.drag + new Vector2(0.0f, Physics2D.gravity.y * ballisticObject.rigidBody.mass)).magnitude.ToString("E2")}\nLocal air pressure:\n{ballisticObject.currentRho.ToString("F3")}kg/m^3\nLocal speed of sound:\n{ballisticObject.currentSpeedOfSound.ToString("F1")}m/s\nKinetic energy: {(0.5 * ballisticObject.rigidBody.mass * ballisticObject.rigidBody.velocity.sqrMagnitude).ToString("F1")}J";
        }
    }

    void SetBullet(BallisticObject b) {
        ballisticObject = b;
    }

    public void Fire() {
        initialPanel.SetActive(false);
        gun.Fire();
        CameraViewer.instance.UpdateField();
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }
    }

    public void NewTimeScale(float newTimeScale) {
        timescale.text = newTimeScale.ToString("F2");
        Time.timeScale = newTimeScale;
    }
    
    public void NewAngle(float newAngle) {
        launchAngle.text = newAngle.ToString("F1");
        gun.angle = newAngle;
    }

    public void SetFireButtonActive() {
        button.interactable = true;
    }

    public void bulletToggled(bool b) {
        fieldToggle.isOn = !b;
        bulletToggle.isOn = b;
        CameraViewer.instance.SetField(fieldToggle.isOn);
    }

    public void fieldToggled(bool b) {
        bulletToggle.isOn = !b;
        fieldToggle.isOn = b;
        CameraViewer.instance.SetField(fieldToggle.isOn);
    }

    public void Reset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NewWindVelocity(float wind) {
        windVelocity.x = wind;
        windVelocityText.text = wind.ToString("F1") + "m/s [->]";
    }

    public void NewGravity(float gravity) {
        Physics2D.gravity = new Vector2(0.0f, -gravity);
        gravityText.text = gravity.ToString("F1") + "N/m";
    }

    public void SetCartridge(int index) {
        if(index != 0) {
            BulletData bulletData = cartridges[index - 1];
            gun.load = bulletData;
            cartridgeInfoText.text = $"Name: {bulletData.name}\n Type: {bulletData.type}\n Year: {bulletData.year}\n Caliber: {bulletData.caliber}mm\n Mass: {bulletData.mass}g\n Muzzle velocity: {bulletData.muzzleVelocity}m/s";
        }
    }
}
