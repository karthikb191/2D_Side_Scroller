using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    [SerializeField]
    private float length;

    public float InstantiatedLayer { set; get; }
    public float Id { set; get; }
    public bool Platform { set; get; }
    public bool Hazard { set; get; }


    private void Start()
    {
        //length = GetComponent<Collider2D>().bounds.size.x;
        //Debug.Log("length: " + length);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
	
    public float GetLength()
    {
        return length;
    }
}
