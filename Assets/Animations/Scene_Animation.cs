using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Animation : MonoBehaviour
{

    public float spd = 0.1f;
    public Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 offset = new Vector2(spd * Time.deltaTime, 0);

        rend.material.mainTextureOffset += offset;

    }
}
