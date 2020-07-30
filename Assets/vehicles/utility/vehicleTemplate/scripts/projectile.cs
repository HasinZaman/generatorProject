using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    bool movementCond = true;

    public GameObject projectileExplotion;

    public Collider collider;
    public TrailRenderer trailRender;

    public bool tracer = false;

    public Vector3 velocity = Vector3.zero;
    public float gravitationalAcceleration = 9.81f;

    [Range(1, 100)]
    public int segments = 1;
    
    private RaycastHit hit;
    
    Color[] temp = new Color[3] { Color.red, Color.blue, Color.green };

    Vector3 distStart;

    Vector3 forwardVector;
    
    private void Start()
    {
        //sets up the intial round settings
        distStart = transform.position;

        float timePerSegment = Time.deltaTime / segments;
        forwardVector = transform.TransformDirection(Vector3.forward * velocity.x * timePerSegment);

        if (tracer)
        {
            trailRender.enabled = tracer;
        }
    }

    //updates the bullets location ever time phyiscs calculations are made
    private void FixedUpdate()
    {
        //if the round is below the minimum of the elavation then the object is destroyed inorder prevent rounds from traveling infintly downwards
        if(this.transform.position.y < 0)
        {
            Destroy(this.gameObject);
        }
        if (movementCond)
        {
            projectileMotion();
        }
    }
    
    //due to the speed of the projectile; the projectile calcuates it's own trajectory inorder to prevent it from phasing through objects
    private void projectileMotion()
    {
        float timePerSegment = Time.deltaTime / segments;
        
        Vector3 startPos = transform.position;

        Vector3 secondLastPos = Vector3.zero;

        float gravitationalDist = 0;
        Vector3 gravityVector;
        
        //checks the position of the projectile travels intercepts an object in it's trajectory
        for (int i1 = 1; i1 <= segments; i1++)
        {
            gravitationalDist = velocity.y * timePerSegment - Convert.ToSingle(0.5f * gravitationalAcceleration * timePerSegment * timePerSegment);
            
            gravityVector = transform.TransformDirection(Vector3.down * -1 * gravitationalDist);

            
            secondLastPos = startPos;
            startPos += forwardVector + gravityVector;

            velocity.y -= gravitationalAcceleration * timePerSegment;
            

            if (Physics.Raycast(startPos, transform.forward + gravityVector, out hit, velocity.x * timePerSegment))
            {
                if (hit.collider != collider)
                {
                    this.transform.position = hit.point;
                    this.transform.rotation = Quaternion.LookRotation(hit.point - startPos);
                    
                    movementCond = false;

                    GameObject explotion = Instantiate(projectileExplotion, this.transform.position, this.transform.rotation);
                    explotion.name = "explotion";

                    if(tracer == true)
                    {
                        explotion.transform.Find("tracer").GetComponent<ParticleSystem>().Play();
                    }

                    Destroy(this.gameObject);

                    return;
                }
            }
            
        }
        
        this.transform.rotation = Quaternion.LookRotation(startPos - secondLastPos);
        this.transform.position = startPos;
    }
    
    //the object destorys it's self when it colldies with another object
    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);

    }
}
