using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedImagePlane : MonoBehaviour
{
   public Sprite[] frames;
   public float fps;

   private Image imageplane;
    private int currentframe = 0;
    private float timer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        //grabs component of image.image
        imageplane = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //sets an fps for the frames traversed and animates the array of sprite images
        float timePerFrames = 1f;
        timer += Time.deltaTime;
        if (timer >= fps) 
        {
            currentframe = (currentframe + 1) % frames.Length;
            imageplane.sprite= frames[currentframe];

            timer =0f;
        
        }
    }
}
