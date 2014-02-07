/*  This file is part of the "3D Tower Defense Starter Kit" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//stores a list of all powerups and triggers their execution
public class PowerUpManager : MonoBehaviour
{
    //list for total battle powerups of each type
    [HideInInspector]
    public List<BattlePowerUp> battlePowerUps = new List<BattlePowerUp>();
    //lists for actual classes
    public List<OffensivePowerUp> battleOffensive = new List<OffensivePowerUp>();
    public List<DefensivePowerUp> battleDefensive = new List<DefensivePowerUp>();
    //current active, selected powerup
    private BattlePowerUp activePowerUp;
    //event fired when a powerup was executed successfully
    public static event Action<BattlePowerUp> powerUpActivated;

    //select powerup based on list indeces
    public void SelectPowerUp(int index)
    {
        //new powerup instance for later comparison
        BattlePowerUp powerUp = null;

        //first search the index in the offensive list of powerups,
        //if the index is greater than this list switch to defensive list
        if (index <= battleOffensive.Count - 1)
            powerUp = battleOffensive[index];
        else
            powerUp = battleDefensive[index - battleOffensive.Count];

        //if we selected the same powerup as before,
        //we deselect it again
        if (activePowerUp == powerUp)
            Deselect();
        else
            //else store this powerup as active selection
            activePowerUp = powerUp;
    }


    //returns whether we have an active selection
    public bool HasSelection()
    {
        if (activePowerUp != null)
            return true;
        else
            return false;
    }


    //return a reference to the active selection
    public BattlePowerUp GetSelection()
    {
        return activePowerUp;
    }


    //unsets the powerup reference
    public void Deselect()
    {
        activePowerUp = null;
    }

    
    //starts a coroutine that executes the selected powerup
    //the corresponding powerup type method is called via overloading
    public void Activate()
    {
        StartCoroutine("ActivatePowerUp", activePowerUp);
    }


    //try to execute offensive powerup
    IEnumerator ActivatePowerUp(OffensivePowerUp powerUp)
    {
        //do not continue if any of these requirements are not met:
        //the powerup is disabled (has cooldown),
        //a single target is set but the powerup has no target,
        //or we have both but the targeted enemy is not alive anymore
        if (!powerUp.enabled || powerUp.singleTarget && !powerUp.target
            || powerUp.singleTarget && powerUp.target && !PoolManager.Props[powerUp.target.name].IsAlive())
        {
            yield break;
        }
        else
            //disable powerup
            powerUp.enabled = false;

        //trigger event notification
        powerUpActivated(powerUp);
        //let the powerup handle its own effects
        powerUp.InstantiateFX();
        //delay option execution
        yield return new WaitForSeconds(powerUp.startDelay);

        //handle 'weaken' option
        if (powerUp.weaken.enabled)
            yield return StartCoroutine(powerUp.Weaken());
        //handle 'explosion' option
        if (powerUp.explosion.enabled)
            yield return StartCoroutine(powerUp.Explosion());
        //handle 'burn' option
        if (powerUp.burn.enabled)
            yield return StartCoroutine(powerUp.Burn());
        //handle 'slow' option
        if (powerUp.slow.enabled)
            yield return StartCoroutine(powerUp.Slow());

        //unset target and position for later reuse
        powerUp.target = null;
        powerUp.position = Vector3.zero;

        //wait until the cooldown is over
        //before re-enabling the powerup
        yield return new WaitForSeconds(powerUp.cooldown);
        powerUp.enabled = true;
    }


    //try to execute defensive powerup
    IEnumerator ActivatePowerUp(DefensivePowerUp powerUp)
    {
        //do not continue if any of these requirements are not met:
        //the powerup is disabled (has cooldown),
        //a single target is set but the powerup has no target
        if (!powerUp.enabled ||
            !powerUp.area && !powerUp.target)
            yield break;
        else
            //disable powerup
            powerUp.enabled = false;

        //trigger event notification
        powerUpActivated(powerUp);
        //let the powerup handle its own effects
        powerUp.InstantiateFX();
        //delay option execution
        yield return new WaitForSeconds(powerUp.startDelay);
        
        //handle boost/buff option
        yield return StartCoroutine(powerUp.BoostTowers());

        //unset target and position for later reuse
        powerUp.target = null;
        powerUp.position = Vector3.zero;

        //wait until the cooldown is over
        //before re-enabling the powerup
        yield return new WaitForSeconds(powerUp.cooldown);
        powerUp.enabled = true;
    }
}