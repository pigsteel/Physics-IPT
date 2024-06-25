using UnityEngine;


[CreateAssetMenu(fileName = "New Bullet Data", menuName = "ScriptableObjects/New Bullet Data", order = 1)]
public class BulletData : ScriptableObject {
    public string name;
    public int year;
    public string type;
    [SerializeField] public float caliber;
    [SerializeField] public float mass;
    [SerializeField] public float muzzleVelocity;
    public Sprite sprite;

    public float GetCaliber() {
        return caliber / 1000;
    }

    public float GetMass() {
        return mass / 1000;
    }

    public float GetMuzzleVelocity() {
        return muzzleVelocity;
    }
}