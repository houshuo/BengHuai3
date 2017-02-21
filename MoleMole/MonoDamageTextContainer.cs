namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoDamageTextContainer : MonoBehaviour
    {
        private List<MonoDamageText> _damageTextPool;
        private const string DAMAGE_TEXT_PATH = "UI/Menus/Widget/InLevel/DamageText";

        private void Awake()
        {
            this._damageTextPool = new List<MonoDamageText>();
        }

        private Vector3 GetRandomDeltaDistance()
        {
            float x = ((UnityEngine.Random.value >= 0.5f) ? ((float) (-1)) : ((float) 1)) * UnityEngine.Random.Range((float) 0.5f, (float) 0.8f);
            return new Vector3(x, ((UnityEngine.Random.value >= 0.5f) ? ((float) (-1)) : ((float) 1)) * UnityEngine.Random.Range((float) 0.1f, (float) 0.2f), 0f);
        }

        public void ShowDamageText(AttackData attackData, BaseMonoEntity attackee)
        {
            if (!GlobalVars.muteDamageText && (attackData.hitLevel != AttackResult.ActorHitLevel.Mute))
            {
                Vector3 hitPoint = attackData.hitCollision.hitPoint;
                if (attackData.damage > 0f)
                {
                    if (attackData.natureDamageRatio < 1f)
                    {
                        this.ShowOneDamageText(DamageType.Restrain, attackData.damage, hitPoint, attackee);
                    }
                    else if (attackData.hitLevel == AttackResult.ActorHitLevel.Critical)
                    {
                        this.ShowOneDamageText(DamageType.Critical, attackData.damage, hitPoint, attackee);
                    }
                    else
                    {
                        this.ShowOneDamageText(DamageType.Normal, attackData.damage, hitPoint, attackee);
                    }
                }
                hitPoint = attackData.hitCollision.hitPoint + this.GetRandomDeltaDistance();
                if (attackData.plainDamage > 0f)
                {
                    this.ShowOneDamageText(DamageType.ElementalNormal, attackData.plainDamage, hitPoint, attackee);
                }
                if (attackData.fireDamage > 0f)
                {
                    this.ShowOneDamageText(DamageType.Fire, attackData.fireDamage, hitPoint, attackee);
                }
                if (attackData.thunderDamage > 0f)
                {
                    this.ShowOneDamageText(DamageType.Thunder, attackData.thunderDamage, hitPoint, attackee);
                }
                if (attackData.iceDamage > 0f)
                {
                    this.ShowOneDamageText(DamageType.Ice, attackData.iceDamage, hitPoint, attackee);
                }
                if (attackData.alienDamage > 0f)
                {
                    this.ShowOneDamageText(DamageType.Allien, attackData.alienDamage, hitPoint, attackee);
                }
            }
        }

        private void ShowOneDamageText(DamageType type, float damage, Vector3 worldPos, BaseMonoEntity attackee)
        {
            MonoDamageText item = null;
            foreach (MonoDamageText text2 in this._damageTextPool)
            {
                if (!text2.gameObject.activeSelf)
                {
                    item = text2;
                    break;
                }
            }
            if (item == null)
            {
                item = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/InLevel/DamageText", BundleType.RESOURCE_FILE)).GetComponent<MonoDamageText>();
                item.transform.SetParent(base.transform, false);
                this._damageTextPool.Add(item);
            }
            else
            {
                item.gameObject.SetActive(true);
            }
            item.SetupView(type, damage, worldPos, attackee);
        }
    }
}

