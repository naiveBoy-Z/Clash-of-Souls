using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSpotEffect : MonoBehaviour
{
    [Header("Select-Effect")]
    public SpriteRenderer spriteRenderer;
    public float changeSpeed = 1f;
    private float spotColorChannel = 1;
    private int direction = -1;
    public bool isSelected = false;

    [Header("Child Object")]
    public GameObject childObject;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isSelected)
        {
            spotColorChannel += changeSpeed * Time.deltaTime * direction;
            if (spotColorChannel < 0.4f) direction = 1;
            else if (spotColorChannel >= 1f)
            {
                spotColorChannel = 1f;
                direction = -1;
            }
            spriteRenderer.color = new Color(spotColorChannel, spotColorChannel, spotColorChannel, 1f);
        }
    }
}
