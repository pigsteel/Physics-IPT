using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticObject : MonoBehaviour
{
    public AnimationCurve g7;
    public AnimationCurve g1;
    public AnimationCurve custom;
    public BulletData bulletData;
    public Rigidbody2D rigidBody;
    public bool plotTrajectory;
    public float machNumber;
    bool fired = false;
    public LineRenderer trajectory;
    public LineRenderer velocityLine;
    public LineRenderer dragLine;
    public LineRenderer fNetLine;

    public float angle;
    public Vector2 drag;

    public float currentSpeedOfSound;
    public float currentRho;



    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Launch(float angle) {
        angle = Mathf.Deg2Rad * angle;
        rigidBody.mass = bulletData.GetMass();
        float initialVelocity = bulletData.GetMuzzleVelocity();
        rigidBody.velocity = new Vector2(initialVelocity * Mathf.Cos(angle), initialVelocity * Mathf.Sin(angle));
        fired = true;
        trajectory.SetPosition(0, transform.position);
    }

    // Update is called once per frame
    public void FixedUpdate() {
        if (plotTrajectory && fired) {
            //Vector3 pos = transform.position + new Vector3(0.0f, 0.0f, 1.0f);
            //trajectory.positionCount++;
            //trajectory.SetPosition(trajectory.positionCount - 1, pos);
            Debug.DrawRay(transform.position, rigidBody.velocity * Time.fixedDeltaTime, Color.green, 1000f);
        }

        (currentRho, currentSpeedOfSound) = CalculateDensityAndSonicSpeed(rigidBody.position.y, 0.0f);

        drag = ForceDrag(rigidBody.velocity);
        // Apply drag
        rigidBody.velocity = rigidBody.velocity - drag * Time.fixedDeltaTime / rigidBody.mass;
        if(rigidBody.velocity.magnitude > 1) rigidBody.rotation = Mathf.Rad2Deg * Mathf.Atan(rigidBody.velocity.y / Mathf.Max(rigidBody.velocity.x, 0.0000001f)) - 90f;
        rigidBody.velocity = rigidBody.velocity + Physics2D.gravity * Time.fixedDeltaTime;
    }

    private void Update() {
        Debug.DrawRay(rigidBody.position, -drag / rigidBody.mass);
        Debug.DrawRay(rigidBody.position, rigidBody.velocity, Color.red);
        Debug.DrawRay(rigidBody.position, -drag + new Vector2(0.0f, Physics2D.gravity.y * rigidBody.mass), Color.yellow);
    }

    Vector2 ForceDrag(Vector2 velocity) {
        return DynamicPressure(velocity) * CrossSectionalArea(bulletData.GetCaliber()) * Coefficient(MachNumber(velocity.magnitude));
    }

    float MachNumber(float speed) {
        machNumber = speed / currentSpeedOfSound;
        return machNumber;
    }

    float Coefficient(float mach) {
        return g7.Evaluate(mach);
    }

    Vector2 DynamicPressure(Vector2 velocity) {
        Vector2 realVel = UIMaster.instance.windVelocity + velocity;
        return 0.5f * currentRho * (realVel.normalized * realVel.sqrMagnitude);
    }

    float CrossSectionalArea(float caliber) {
        return Mathf.PI * Mathf.Pow(caliber / 2f, 2f);
    }

    public static (float, float) CalculateDensityAndSonicSpeed(double altSI, double temperature) {

        //converting tempSI to tempRawSI to accomodate the offset case
        //additional loop needed here to determine whether the value is a delta (offset) or actual (OAT)        
        var tempRawSI = temperature;

        // define the various arrays required
        var altitudeArray = new double[] { 0, 11000, 20000, 32000, 47000, 51000, 71000, 84852 };
        var presRelsArray = new double[] { 1, 2.23361105092158e-1, 5.403295010784876e-2, 8.566678359291667e-3, 1.0945601337771144e-3, 6.606353132858367e-4, 3.904683373343926e-5, 3.6850095235747942e-6 };
        var tempsArray = new double[] { 288.15, 216.65, 216.65, 228.65, 270.65, 270.65, 214.65, 186.946 };
        var tempGradArray = new double[] { -6.5f, 0, 1, 2.8f, 0, -2.8f, -2, 0 };

        var i = 0;
        while (altSI > altitudeArray[i + 1]) {
            i = i + 1;
        }

        // i now defines the array position that I require for the calc
        var alts = altitudeArray[i];
        var presRels = presRelsArray[i];
        var temps = tempsArray[i];
        var tempGrad = tempGradArray[i] / 1000;

        var deltaAlt = altSI - alts;
        var stdTemp = temps + (deltaAlt * tempGrad); // this is the standard temperature at STP
        var tempSI = tempRawSI + stdTemp;

        // Now for the relative pressure calc:
        //define constants
        var airMol = 28.9644;
        var rhoSL = 1.225; // kg/m3
        var pSL = 101325; // Pa
        var tSL = 288.15; //K
        var gamma = 1.4;
        var g = 9.80665; // m/s2
        var stdTempGrad = -0.0065; // deg K/m 
        var rGas = 8.31432; //kg/Mol/K
        var R = 287.053;
        var gMR = g * airMol / rGas;
        var dryMM = 0.0289644; //kg/mol

        var relPres = 0d;
        if (Math.Abs(tempGrad) < 1e-10) {
            relPres = presRels * Math.Exp(-1 * gMR * deltaAlt / 1000 / temps);
        }
        else {
            relPres = presRels * Math.Pow(temps / stdTemp, gMR / tempGrad / 1000);
        }

        // Now I can calculate the results:

        var sonicSI = Math.Sqrt(gamma * R * tempSI);
        var pressureSI = pSL * relPres;
        var rhoSI = rhoSL * relPres * (tSL / tempSI);
        var sigma = rhoSI / rhoSL;

        var denAltSIpow = 1 - Math.Pow(((pressureSI / pSL) / (tempSI / tSL)), -1 * stdTempGrad * rGas / (g * dryMM + stdTempGrad * rGas));


        var finalRho = rhoSI;
        var finalSonic = sonicSI;
        return ((float)finalRho, (float)sonicSI);
    }
}
