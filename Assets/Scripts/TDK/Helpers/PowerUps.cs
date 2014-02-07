/*  This file is part of the "3D Tower Defense Starter Kit" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//main powerup class, that stores all general properties.
//other battle powerups are derived from this class
[System.Serializable]
public class BattlePowerUp
{
    public string name = "";        //powerup name to display
    public string description = ""; //description to display at runtime
    public bool enabled = true;     //whether this powerup is available
    public float startDelay;        //delay before applying activation methods
    public float cooldown;          //cooldown before a reuse is possible
    public UILabel timerText;       //label for displaying remaining cooldown time
    public int targets = 1;         //how many targets to affect
    public AudioClip sound;         //sound to play on activation
    public GameObject fx;           //effect to spawn on activation

    [HideInInspector]
    public Transform target;        //clicked target transform
    [HideInInspector]
    public Vector3 position;        //clicked world position


    //instantiate particle fx and play sound, if set
    public void InstantiateFX()
    {
        if (fx)
            PoolManager.Pools["Particles"].Spawn(fx, position, Quaternion.identity);

        if (sound)
            AudioManager.Play(sound, position);
    }


    //sort target colliders based on distance
    public List<Collider> SortByDistance(List<Collider> cols)
    {
        //create new sorted list, but return if empty
        List<Collider> sortedCols = new List<Collider>();
        if (cols.Count == 0) return sortedCols;

        //create temporary list for modification purposes
        //populate with default values
        List<Collider> tempCols = new List<Collider>();
        for (int i = 0; i < cols.Count; i++)
            tempCols.Add(cols[i]);

        //iterate over all colliders
        for (int i = 0; i < cols.Count; i++)
        {
            //float value for storing the nearest distance,
            //appropriate collider variable and corresponding index
            float nearest = float.MaxValue;
            Collider col = null;
            int index = 0;
            //iterate over temporary list with colliders
            for (int j = 0; j < tempCols.Count; j++)
            {
                //skip this collider/enemy if it died already
                if (tempCols[j].gameObject.layer == SV.enemyLayer
                   && !PoolManager.Props[tempCols[j].gameObject.name].IsAlive())
                    continue;

                //calculate distance to the collider
                float distance = (tempCols[j].transform.position - position).sqrMagnitude;
                //if the distance is lower than the previous nearest distance,
                //and the enemy is still alive, set this collider as target,
                //overwrite nearest value and set index. redo for all colliders
                if (distance < nearest)
                {
                    col = tempCols[j];
                    nearest = distance;
                    index = j;
                }
            }

            //add collider to the sorted list, but don't add an empty collider
            //and remove it from the temporary list for another iteration
            if (col == null) break;
            sortedCols.Add(col);
            tempCols.RemoveAt(index);
        }

        //cap sorted list if count is greater than actual target amount
        if (sortedCols.Count > targets)
            sortedCols.RemoveRange(targets, sortedCols.Count - targets);
        //return sorted list
        return sortedCols;
    }
}


//offensive powerups, affecting enemies.
//derived from BattlePowerUp
[System.Serializable]
public class OffensivePowerUp : BattlePowerUp
{
    public TDValue damageType;                      //damage type (fix/percentual)
    public float damage;                            //damage value
    public bool singleTarget = false;               //toggle for single target only
    public Explosion explosion = new Explosion();   //explosion instance
    public Burn burn = new Burn();                  //burn instance
    public Slow slow = new Slow();                  //slow instance
    public Weakening weaken = new Weakening();      //weaken instance


    //same as in Projectile.cs, just with little modifications
    //to the trigger position and collider sorting based on distance
    //please refer to the projectile script for comments
    public IEnumerator Explosion()
    {
        List<Collider> cols = new List<Collider>(Physics.OverlapSphere(position, explosion.radius, SV.enemyMask));
        cols = SortByDistance(cols);

        for (int i = 0; i < cols.Count; i++)
        {
            Properties targetProp = PoolManager.Props[cols[i].name];
            if (damageType == TDValue.fix)
                targetProp.Hit(damage * explosion.factor);
            else
                targetProp.Hit(targetProp.maxhealth * damage * explosion.factor);

            if (explosion.fx)
                PoolManager.Pools["Particles"].Spawn(explosion.fx, cols[i].transform.position, Quaternion.identity);
        }

        yield return null;
    }


    //same as in Projectile.cs, just with little modifications
    //to the trigger position and collider sorting based on distance
    //please refer to the projectile script for comments
    public IEnumerator Burn()
    {
        List<Collider> cols = new List<Collider>();

        if (burn.area)
            cols = new List<Collider>(Physics.OverlapSphere(position, burn.radius, SV.enemyMask));
        else if (target)
            cols.Add(target.collider);

        cols = SortByDistance(cols);

        for (int i = 0; i < cols.Count; i++)
        {
            Transform colTrans = cols[i].transform;
            Properties targetProp = PoolManager.Props[colTrans.name];

            if (!targetProp.IsAlive())
                continue;

            float[] vars = new float[3];

            if (damageType == TDValue.fix)
                vars[0] = damage * burn.factor;
            else
                targetProp.Hit(targetProp.maxhealth * damage * burn.factor);
            vars[1] = burn.time;
            vars[2] = burn.frequency;

            if (!burn.stack)
                targetProp.StopCoroutine("DamageOverTime");
            targetProp.StartCoroutine("DamageOverTime", vars);

            if (!burn.fx) continue;

            Transform dotEffect = null;
            foreach (Transform child in colTrans)
            {
                if (child.name.Contains(burn.fx.name))
                    dotEffect = child;
            }

            if (!dotEffect)
            {
                dotEffect = PoolManager.Pools["Particles"].Spawn(burn.fx, colTrans.position, Quaternion.identity).transform;
                dotEffect.parent = colTrans;
            }
        }

        yield return null;
    }


    //same as in Projectile.cs, just with little modifications
    //to the trigger position and collider sorting based on distance
    //please refer to the projectile script for comments
    public IEnumerator Slow()
    {
        List<Collider> cols = new List<Collider>();

        if (slow.area)
            cols = new List<Collider>(Physics.OverlapSphere(position, slow.radius, SV.enemyMask));
        else if (target)
            cols.Add(target.collider);

        cols = SortByDistance(cols);

        for (int i = 0; i < cols.Count; i++)
        {
            Properties targetProp = PoolManager.Props[cols[i].name];
            targetProp.Slow(slow.time, slow.factor);

            if (slow.fx)
                PoolManager.Pools["Particles"].Spawn(slow.fx, cols[i].transform.position, Quaternion.identity);
        }

        yield return null;
    }


    //weakens the enemy by a percentual value
    //(percentual damage to health/shield or both)
    public IEnumerator Weaken()
    {
        //initialize list of targets
        List<Collider> cols = new List<Collider>();

        if (weaken.area)
            //if the powerup has an area of effect, it should just hurt layer 'SV.enemyMask' (8 = Enemies)
            //check enemies in range (within the radius) and store their colliders
            cols = new List<Collider>(Physics.OverlapSphere(position, weaken.radius, SV.enemyMask));
        else if (target)
            //without area of effect, we only store the clicked target collider
            cols.Add(target.collider);

        //sort colliders based on distance
        cols = SortByDistance(cols);

        //loop through colliders
        for (int i = 0; i < cols.Count; i++)
        {
            //if an weaken effect is set,
            //activate/instantiate it at the enemy's position
            if (weaken.fx)
                PoolManager.Pools["Particles"].Spawn(weaken.fx, cols[i].transform.position, Quaternion.identity);

            //get enemy properties from the PoolManager dictionary
            Properties targetProp = PoolManager.Props[cols[i].name];
            //cache health value
            float healthValue = targetProp.health;
            //initialize shield value and set it
            //if shield option of this enemy is enabled
            float shieldValue = 0;
            if (targetProp.shield.enabled)
                shieldValue = targetProp.shield.value;

            //differentiate between weaken type
            switch (weaken.type)
            {
                //only do damage to the shield value
                case Weakening.Type.shieldOnly:
                    if (shieldValue > 0)
                        targetProp.Hit(shieldValue * weaken.factor);
                    break;
                //only do damage to the health, ignoring shield value:
                //first we include the total shield value as damage value,
                //then reset it afterwards and update the unit bar frames
                case Weakening.Type.healthOnly:
                    targetProp.Hit(shieldValue + healthValue * weaken.factor);
                    targetProp.shield.value = shieldValue;
                    targetProp.SetUnitFrame();
                    break;
                //do damage to both the health and shield value simultaneously
                case Weakening.Type.all:
                    targetProp.Hit(shieldValue + healthValue * weaken.factor);
                    targetProp.shield.value = shieldValue * (1f - weaken.factor);
                    targetProp.SetUnitFrame();
                    break;
            }
        }

        yield return null;
    }
}


//defensive powerups, affecting towers.
//derived from BattlePowerUp
[System.Serializable]
public class DefensivePowerUp : BattlePowerUp
{
    public GameObject buffFx;       //particle fx to instantiate at the tower position
    public float duration = 1;      //buff duration
    public bool area;               //whether this powerup affects an area
    public float radius;            //radius for area collision
    public Buff buff = new Buff();  //buff class instance


    //modifiable properties
    [System.Serializable]
    public class Buff
    {
        //TowerBase variables
        public float shotAngle = 1;
        public float controlMultiplier = 1;

        //Upgrade variables
        public float radius = 1;
        public float damage = 1;
        public float shootDelay = 1;
        public int targetCount = 0;
    }


    //on execution, this method boosts the towers triggered
    public IEnumerator BoostTowers()
    {
        //initialize list of targets
        List<Collider> cols = new List<Collider>();

        if (area)
            //area collision physics cast,
            //at the clicked position with defined radius against towers
            cols = new List<Collider>(Physics.OverlapSphere(position, radius, SV.towerMask));
        else if (target)
            //without area of effect, we only store the clicked tower target collider
            cols.Add(target.collider);

        //sort colliders based on distance
        cols = SortByDistance(cols);

        //loop through colliders and apply powerup to them
        for (int i = 0; i < cols.Count; i++)
        {
            TowerBase towerBase = cols[i].GetComponent<TowerBase>();
            towerBase.ApplyPowerUp(this);
        }

        yield return null;
    }
}