using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour {

    //GameObject order determines activation
    [SerializeField] Transform chargesTransform;

    [SerializeField] float explosionDelay = 1.5f;
    [SerializeField] float explosionLifeTime = 0.2f;

    private int chargeCount = 8;
    private int maxCharges = 8;

    private float shockWaveRadius = 3.5f;
    private float power = 300F;

    bool canDoExplosion = true;

    /// <summary>
    /// Do shockwave and remove charge
    /// </summary>
    public void shockWave()
    {
        if(chargeCount > 0 && gameObject.activeSelf)
        {
            minCharge(1);
            canDoExplosion = false;
            StartCoroutine(explosionRoutine());
        }
    }

    private IEnumerator explosionRoutine()
    {
        GameObject explosion = Instantiate(Game.instance.playerExplosionPrefab);
        explosion.transform.position = this.transform.position;
        StartCoroutine(inputDelayRoutine());

        yield return new WaitForSeconds(explosionLifeTime);
        
        Destroy(explosion);
    }

    private IEnumerator inputDelayRoutine()
    {
        yield return new WaitForSeconds(explosionDelay);
        canDoExplosion = true;
    }

    /// <summary>
    /// Add a charge
    /// </summary>
    /// <param name="count"></param>
    public void addCharge(int count)
    {
        //When reaching goal score on max charges
        //you will receive some bonus score
        int newChargeCount = chargeCount + count;
        if (newChargeCount < maxCharges)
        {            
            setCharges(newChargeCount);
            chargeCount = newChargeCount;
        }
        else
        {
            Game.instance.addScore(777);
        }
    }

    /// <summary>
    /// Remove a charge
    /// </summary>
    /// <param name="count"></param>
    public void minCharge(int count)
    {
        int newChargeCount = chargeCount - count;
        if (newChargeCount > 0)
        {            
            setCharges(newChargeCount);
            chargeCount = newChargeCount;
        }
    }

    /// <summary>
    /// TODO: Draw a circle of gameobjects (spheres) around player?
    /// If given count is smaller/larger than given count enable/disable charge objects
    /// </summary>
    /// <param name="count"></param>
    private void setCharges(int newChargeCount)
    {                
        if (newChargeCount > chargeCount)
        {
            //Number of charges to be enabled
            int enabledCount = newChargeCount - chargeCount;
            setActiveCharges(chargeCount, 0, -1, enabledCount, true);
        }
        else
        {
            //Number of charges to be disabled
            int disabledCount = chargeCount - newChargeCount;
            setActiveCharges(0, disabledCount, 1, disabledCount, false);
        }
    }

    /// <summary>
    /// Loop through charges and enable/disable by given amount
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <param name="step"></param>
    /// <param name="count"></param>
    /// <param name="active"></param>
    private void setActiveCharges(int startIndex, int endIndex, int step, int count, bool active)
    {
        int i = startIndex;
        int childCount = chargesTransform.childCount;

        bool condition = step > 0 ? i < endIndex : i > endIndex;
        for (i = startIndex; condition; i += step)
        {
            if (count == 0 || i > childCount) return;
            Transform charge = chargesTransform.GetChild((int)i);
            bool checkActive = active ? !charge.gameObject.activeSelf : charge.gameObject.activeSelf;        
            if (checkActive)
            {
                charge.gameObject.SetActive(active);
                count--;
            }
        }
    }
}
