using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class muzzleFlash : MonoBehaviour
{
    //muzzle flash
    private System.Random random = new System.Random();

    public GameObject flashMesh;
    public GameObject light;

    public bool flashCond = false;
    public float flashTime = 0.1f;
    private float flashCounter = 0;
    public float[] muzzleFlashBlendShapes = new float[9];
    
    public Vector3[] scaleRange;
    public float[] lightIntesnityRange;

    public float scaleChangeRate;
    public float rotationChangeRate;
    public float[] muzzleFlashBlendShapesChangeRate;


    public AudioSource audioSource;
    public AudioClip[] audioClips;
    
    // Start is called before the first frame update
    void Start()
    {
        //creates a random and unque muzzle flash
        muzzleFlashBlendShapes[0] = random.Next(20, 100);
        flashMesh.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, muzzleFlashBlendShapes[0]);

        for (int i1 = 1; i1 < muzzleFlashBlendShapes.Length; i1++)
        {
            muzzleFlashBlendShapes[i1] = random.Next(20, 100);
            flashMesh.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(i1, muzzleFlashBlendShapes[i1]);
        }

        Vector3 rotation = this.transform.localEulerAngles;
        rotation.z = random.Next(0, 360);
        this.transform.localEulerAngles = rotation;

        flashMesh.transform.localScale = new Vector3
            (
                Convert.ToSingle(random.NextDouble() * (scaleRange[1].x - scaleRange[0].x)) + scaleRange[0].x,
                Convert.ToSingle(random.NextDouble() * (scaleRange[1].y - scaleRange[0].y)) + scaleRange[0].y,
                Convert.ToSingle(random.NextDouble() * (scaleRange[1].z - scaleRange[0].z)) + scaleRange[0].z
            );
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //checks if the muzzle flash is active
        if(flashCond != flashMesh.GetComponent<SkinnedMeshRenderer>().enabled)
        {
            flashMesh.GetComponent<SkinnedMeshRenderer>().enabled = flashCond;
            light.GetComponent<Light>().enabled = flashCond;
            this.Start();
            audioSource.clip = audioClips[random.Next(0, audioClips.Length - 1)];
            audioSource.Play();
        }

        //checks if the muzzle flash is over if it has to contiue to procdural animaition
        if(flashCond && Time.deltaTime > 0)
        {

            if (flashTime - flashCounter <= 0)
            {
                flashCond = false;
                flashCounter = 0;
            }
            else
            {
                //procedual animation shit
            }

            flashCounter+=Time.deltaTime;
        }
    }

    //draws the muzzle flash where it would be on the editor
    private void OnDrawGizmos()
    {
        if (Application.isPlaying ==  false)
        {
            Gizmos.DrawMesh(flashMesh.GetComponent<SkinnedMeshRenderer>().sharedMesh);
        }
    }
}
