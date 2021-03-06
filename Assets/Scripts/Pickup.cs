using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public enum PickupType { 
        Score,       //Green
        DoubleScore, //Yellow
        Quicken,     //Purple
        Multiplier   //Blue
    };
    public PickupType type = PickupType.Score;

    public Material greenMaterial;
    public Material goldmaterial;
    public Material purpleMaterial;
    public Material redMaterial;
    public Material blueMaterial;

    //public Collider collider;   
    private Material colliderMaterial;

    private int scoreValue = 33;
    private Renderer theRenderer;

    private void OnEnable()
    {
        theRenderer = GetComponentInChildren<Renderer>();
    }

    public void setPickupType(PickupType type)
    {
        this.type = type;
        switch(type)
        {            
            case PickupType.Score: applyTypeProperties(greenMaterial, 33); break;       //Basic score pickup
            case PickupType.Quicken: applyTypeProperties(purpleMaterial, 77); break;    //Speed pickup
            case PickupType.DoubleScore: applyTypeProperties(goldmaterial, 99); break;  //Double all scores
            case PickupType.Multiplier: applyTypeProperties(blueMaterial, 0); break;    //Score mulitplier
        }
        theRenderer.material.SetColor("_Emission", theRenderer.material.color.gamma);
    }

    private void applyTypeProperties(Material material, int score)
    {
        theRenderer.material = material;
        scoreValue = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {            
            Game.instance.addScore(scoreValue);
            if (type == PickupType.Quicken) Game.instance.quicken();
            Destroy(gameObject);
        }
    }
}
