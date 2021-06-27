using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("Idle Animations")]
    public string Right_Hand_Idle;
    public string Left_Handle_Idle;
    public string Two_Hand_Idle;

    [Header("One Handed Attack Animation")]
    public string OH_Light_Attack_1;
    public string OH_Light_Attack_2;
    public string OH_Heavy_Attack_1;
    public string OH_Heavy_Attack_2;
    public string OH_Run_Attack_01;
    public string OH_Run_Attack_02;

    [Header("Two Handed Attack Animation")]
    public string TH_Light_Attack_1;
    public string TH_Light_Attack_2;
    public string TH_Light_Attack_3;
    public string TH_Heavy_Attack_1;
    public string TH_Run_Attack_01;
    public string TH_Run_Attack_02;

    [Header("Stamina Costs")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;
}
