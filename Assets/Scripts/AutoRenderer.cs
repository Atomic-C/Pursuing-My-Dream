using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRenderer : MonoBehaviour
{
    public Sprite spriteChoosen;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = spriteChoosen;
    }

}
