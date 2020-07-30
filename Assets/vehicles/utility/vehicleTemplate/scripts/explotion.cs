using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explotion : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particles;

    bool particleEndCond = true;

    // Start is called before the first frame update
    void Start()
    {
        //gets every particle in the explotion template   
        particles = this.transform.GetComponentsInChildren<ParticleSystem>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //checks if the particle effects are still playing and when its over; the game object is removed from the game
        for(int i1 = 0; i1 < particles.Length; i1++)
        {
            if (particles[i1].isPlaying)
            {
                particleEndCond = false;
                break;
            }
        }

        if (particleEndCond)
        {
            Destroy(this.gameObject);
        }
    }
}
