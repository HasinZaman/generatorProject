using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public bool controlCond = true;

    public vehicle v;

    public int currentTurret;

    private VehicleControler c;

    bool toggleLight, brake, shoot;
    Vector2 movement, turretMovement;
    float gear, turretChange;
    
    void Awake()
    {
        c = new VehicleControler();
    }

    void OnEnable()
    {
        c.Enable();
    }

    void OnDisable()
    {
        c.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlCond)
        {
            //update controls
            toggleLight = c.land.toggleLight.ReadValue<float>() == 1;
            movement = c.land.movement.ReadValue<Vector2>();
            gear = c.land.gear.ReadValue<float>();
            brake = c.land.brake.ReadValue<float>() == 1f;

            turretMovement = c.land.turretMovement.ReadValue<Vector2>();
            shoot = c.land.shoot.ReadValue<float>() == 1f;
            turretChange = c.land.turretChange.ReadValue<float>();
            
            //light toggle
            if (toggleLight)
            {
                v.toggleLights();
            }

            //gas
            switch (movement.y)
            {
                //forward
                case 1:
                    v.moveCond = true;
                    v.setTargetSpeed(v.speeds[v.gearPos]);
                    if (v.velocityRelativeToForward.z >= 0)
                    {
                        v.brakeCond = false;
                    }
                    else
                    {
                        v.brakeCond = true;
                    }
                    break;
                //backward
                case -1:
                    v.moveCond = true;
                    v.setTargetSpeed(v.speeds[v.gearPos]);
                    if (v.velocityRelativeToForward.z <= 0)
                    {
                        v.brakeCond = false;
                    }
                    else
                    {
                        v.brakeCond = true;
                    }
                    break;
                //neutral
                case 0:
                    v.moveCond = false;
                    v.setTargetSpeed(0);
                    break;
            }

            //steering
            switch(movement.x)
            {
                //right
                case 1f:
                    v.targetSteering = 360;
                    break;
                //left
                case -1f:
                    v.targetSteering = -360;
                    break;
                //forward
                case 0:
                    v.targetSteering = 0;
                    break;
            }

            //brake
            if(brake)
            {
                v.brakeCond = true;
            }
            else
            {
                v.brakeCond = false;
            }

            //gear change
            //gear up
            if(gear == 1)
            {
                v.gearPos = v.setGear(Convert.ToInt32(v.gearPos + gear));
            }
            //gear down
            else if(gear == -1)
            {
                v.gearPos = v.setGear(Convert.ToInt32(v.gearPos + gear));
            }

            //turrets
            //changes selected Turret
            if (turretChange != 0)
            {
                if (turretChange + currentTurret >= v.turrets.Length)
                {
                    currentTurret = -1;
                }
                else if(turretChange + currentTurret < -1)
                {
                    currentTurret = v.turrets.Length - 1;
                }
                else
                {
                    currentTurret += Convert.ToInt32(turretChange);
                }
            }
            
            //move turret horizontally
            if (turretMovement.x != 0 && currentTurret != -1)
            {
                switch (v.turrets[currentTurret].GetComponent<turret>().turretRotation.rotationType)
                {
                    case 0:
                        v.turrets[currentTurret].GetComponent<turret>().turretRotation.full.targetAngleSet(v.turrets[currentTurret].GetComponent<turret>().turretRotation.full.targetAngle + turretMovement.x / 10);
                        break;
                    case 1:
                        v.turrets[currentTurret].GetComponent<turret>().turretRotation.limit.targetAngleSet(v.turrets[currentTurret].GetComponent<turret>().turretRotation.limit.targetAngle + turretMovement.x / 10);
                        break;
                }
            }

            //move the barrel elevation
            if (turretMovement.y != 0 && currentTurret != -1)
            {
                switch (v.turrets[currentTurret].GetComponent<turret>().barrelElevation.rotationType)
                {
                    case 0:
                        v.turrets[currentTurret].GetComponent<turret>().barrelElevation.full.targetAngleSet(v.turrets[currentTurret].GetComponent<turret>().barrelElevation.full.targetAngle + turretMovement.y / 10);
                        break;
                    case 1:
                        v.turrets[currentTurret].GetComponent<turret>().barrelElevation.limit.targetAngleSet(v.turrets[currentTurret].GetComponent<turret>().barrelElevation.limit.targetAngle + turretMovement.y / 10);
                        break;
                }
            }
            
            //fire turret
            if(shoot)
            {
                v.turrets[currentTurret].GetComponent<turret>().fire();
            }
        }
    }
}
