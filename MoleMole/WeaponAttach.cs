namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class WeaponAttach
    {
        private static Transform AttachPartToAttachPoint(IWeaponAttacher avatar, string partPath, string attachPoint, Transform protoTrans)
        {
            Transform transform = avatar.GetAttachPoint(attachPoint);
            GameObject gameObject = protoTrans.Find(partPath).gameObject;
            if (gameObject.GetComponent<MeshFilter>() == null)
            {
                transform.GetComponent<MeshFilter>().sharedMesh = null;
                return transform;
            }
            transform.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            transform.GetComponent<MeshRenderer>().sharedMaterials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
            return transform;
        }

        public static void AttachWeaponMesh(ConfigWeapon weaponConfig, IWeaponAttacher avatar, Transform weaponProtoTrans, string avatarType)
        {
            weaponConfig.Attach.GetAttachHandler()(weaponConfig.Attach, weaponProtoTrans, avatar, avatarType);
        }

        public static Transform[] BronyaAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher bronya, string avatarType)
        {
            Transform[] transformArray = new Transform[2];
            DeleteAttachPointChildren(bronya, "WeaponLeftArmIn", true);
            DeleteAttachPointChildren(bronya, "WeaponLeftArmOut", true);
            transformArray[0] = AttachPartToAttachPoint(bronya, "GunIn", "WeaponLeftArmIn", weaponProtoTrans);
            transformArray[1] = AttachPartToAttachPoint(bronya, "GunOut", "WeaponLeftArmOut", weaponProtoTrans);
            InstantiateChildrenAndAttach(weaponProtoTrans.Find("GunIn"), transformArray[0], true);
            InstantiateChildrenAndAttach(weaponProtoTrans.Find("GunOut"), transformArray[1], true);
            Material[] sharedMaterials = bronya.gameObject.transform.Find("MC_Body").GetComponent<Renderer>().sharedMaterials;
            Material targetMaterial = null;
            foreach (Material material2 in sharedMaterials)
            {
                if (material2.shader.name == "miHoYo/Character/Machine")
                {
                    targetMaterial = material2;
                    break;
                }
            }
            if (targetMaterial != null)
            {
                BronyaCopyMcBodyMaterial(targetMaterial, transformArray[0]);
                BronyaCopyMcBodyMaterial(targetMaterial, transformArray[1]);
            }
            int layer = LayerMask.NameToLayer("Weapon");
            SetRendererLayer(transformArray[0], layer);
            SetRendererLayer(transformArray[1], layer);
            MoveAvatarAttachPoint(bronya, "GunIn/GunPointAttach", "GunPoint", weaponProtoTrans);
            bronya.gameObject.GetComponent<Animator>().Rebind();
            return transformArray;
        }

        private static void BronyaCopyMcBodyMaterial(Material targetMaterial, Transform attachTrans)
        {
            Color color = targetMaterial.GetColor("_Color");
            Color color2 = targetMaterial.GetColor("_OutlineColor");
            Texture texture = targetMaterial.GetTexture("_SPTex");
            Texture texture2 = targetMaterial.GetTexture("_SPNoiseTex");
            float @float = targetMaterial.GetFloat("_SPNoiseScaler");
            float num2 = targetMaterial.GetFloat("_SPIntensity");
            float num3 = targetMaterial.GetFloat("_SPTransition");
            Color color3 = targetMaterial.GetColor("_SPTransitionColor");
            Color color4 = targetMaterial.GetColor("_SPOutlineColor");
            foreach (Renderer renderer in attachTrans.GetComponentsInChildren<Renderer>())
            {
                Material[] materials;
                if (Application.isPlaying)
                {
                    materials = renderer.materials;
                }
                else
                {
                    materials = renderer.sharedMaterials;
                }
                foreach (Material material in materials)
                {
                    if (material.shader.name == "miHoYo/Character/Machine")
                    {
                        material.SetColor("_Color", color);
                        material.SetColor("_OutlineColor", color2);
                        material.SetTexture("_SPTex", texture);
                        material.SetTexture("_SPNoiseTex", texture2);
                        material.SetFloat("_SPNoiseScaler", @float);
                        material.SetFloat("_SPIntensity", num2);
                        material.SetFloat("_SPTransition", num3);
                        material.SetColor("_SPTransitionColor", color3);
                        material.SetColor("_SPOutlineColor", color4);
                    }
                }
            }
        }

        public static void BronyaDetachHandler(ConfigWeaponAttach config, IWeaponAttacher bronya, string avatarType)
        {
            DetachWeaponAttachPoint(bronya, "WeaponLeftArmIn");
            DetachWeaponAttachPoint(bronya, "WeaponLeftArmOut");
            DeleteAttachPointChildren(bronya, "WeaponLeftArmIn", true);
            DeleteAttachPointChildren(bronya, "WeaponLeftArmOut", true);
        }

        public static void BronyaRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType)
        {
            BronyaWeaponAttach attach = (BronyaWeaponAttach) config.Attach;
            if (!string.IsNullOrEmpty(attach.WeaponEffectPattern))
            {
                SetTransformParentAndReset(Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(attach.WeaponEffectPattern, avatar).transform, avatar.GetAttachPoint("GunPoint"));
            }
        }

        private static void CopyTransformLocalProperties(Transform from, Transform to)
        {
            to.localPosition = from.localPosition;
            to.localRotation = from.localRotation;
            to.localScale = from.localScale;
        }

        private static void DeleteAttachPointChildren(IWeaponAttacher avatar, string attachPoint, bool onlyMeshChild = true)
        {
            Transform transform = avatar.GetAttachPoint(attachPoint);
            int index = 0;
            while (transform.childCount > index)
            {
                if (onlyMeshChild && (transform.GetChild(index).GetComponent<MeshRenderer>() == null))
                {
                    index++;
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(index).gameObject);
                }
            }
        }

        private static void DetachWeaponAttachPoint(IWeaponAttacher avatar, string attachPoint)
        {
            Transform transform = avatar.GetAttachPoint(attachPoint);
            transform.GetComponent<MeshFilter>().sharedMesh = null;
            transform.GetComponent<MeshRenderer>().sharedMaterials = new Material[0];
        }

        public static Transform[] FukaAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher fuka, string avatarType)
        {
            DeleteAttachPointChildren(fuka, "WeaponTail", true);
            Transform[] transformArray = new Transform[] { AttachPartToAttachPoint(fuka, "Tail", "WeaponTail", weaponProtoTrans) };
            InstantiateChildrenAndAttach(weaponProtoTrans.Find("Tail"), transformArray[0], true);
            return transformArray;
        }

        public static void FukaDetachHandler(ConfigWeaponAttach config, IWeaponAttacher fuka, string avatarType)
        {
            DetachWeaponAttachPoint(fuka, "WeaponTail");
            DeleteAttachPointChildren(fuka, "WeaponTail", true);
        }

        public static void FukaRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity fuka, string avatarType)
        {
        }

        public static Transform[] HimekoAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher himeko, string avatarType)
        {
            DeleteAttachPointChildren(himeko, "WeaponRightHand", true);
            Transform[] transformArray = new Transform[] { AttachPartToAttachPoint(himeko, "Sword", "WeaponRightHand", weaponProtoTrans) };
            InstantiateChildrenAndAttach(weaponProtoTrans.Find("Sword"), transformArray[0], true);
            return transformArray;
        }

        public static void HimekoDetachHandler(ConfigWeaponAttach config, IWeaponAttacher himeko, string avatarType)
        {
            DetachWeaponAttachPoint(himeko, "WeaponRightHand");
            DeleteAttachPointChildren(himeko, "WeaponRightHand", true);
        }

        public static void HimekoRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity himeko, string avatarType)
        {
        }

        private static void InstantiateChildrenAndAttach(Transform weaponProtoTrans, Transform weaponTrans, bool onlyMeshChild = true)
        {
            for (int i = 0; i < weaponProtoTrans.childCount; i++)
            {
                GameObject gameObject = weaponProtoTrans.GetChild(i).gameObject;
                if (!onlyMeshChild || (gameObject.GetComponent<MeshRenderer>() != null))
                {
                    GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                    obj3.transform.SetParent(weaponTrans, false);
                    obj3.transform.localPosition = gameObject.transform.localPosition;
                    obj3.transform.localRotation = gameObject.transform.localRotation;
                    obj3.transform.localScale = gameObject.transform.localScale;
                }
            }
        }

        public static Transform[] KianaAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher kiana, string avatarType)
        {
            DeleteAttachPointChildren(kiana, "WeaponLeftHand", true);
            DeleteAttachPointChildren(kiana, "WeaponRightHand", true);
            Transform[] transformArray = new Transform[] { AttachPartToAttachPoint(kiana, "LeftPistol", "WeaponLeftHand", weaponProtoTrans), AttachPartToAttachPoint(kiana, "RightPistol", "WeaponRightHand", weaponProtoTrans) };
            InstantiateChildrenAndAttach(weaponProtoTrans.Find("LeftPistol"), transformArray[0], true);
            InstantiateChildrenAndAttach(weaponProtoTrans.Find("RightPistol"), transformArray[1], true);
            MoveAvatarAttachPoint(kiana, "LeftPistol/LeftGunPoint", "LeftGunPoint", weaponProtoTrans);
            MoveAvatarAttachPoint(kiana, "RightPistol/RightGunPoint", "RightGunPoint", weaponProtoTrans);
            return transformArray;
        }

        public static void KianaDetachHandler(ConfigWeaponAttach config, IWeaponAttacher kiana, string avatarType)
        {
            DetachWeaponAttachPoint(kiana, "WeaponLeftHand");
            DetachWeaponAttachPoint(kiana, "WeaponRightHand");
            DeleteAttachPointChildren(kiana, "WeaponLeftHand", true);
            DeleteAttachPointChildren(kiana, "WeaponRightHand", true);
        }

        public static void KianaRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType)
        {
            KianaWeaponAttach attach = (KianaWeaponAttach) config.Attach;
            if (!string.IsNullOrEmpty(attach.WeaponEffectPattern))
            {
                GameObject obj2 = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(attach.WeaponEffectPattern, avatar);
                GameObject obj3 = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(attach.WeaponEffectPattern, avatar);
                SetTransformParentAndReset(obj2.transform, avatar.GetAttachPoint("LeftGunPoint"));
                SetTransformParentAndReset(obj3.transform, avatar.GetAttachPoint("RightGunPoint"));
            }
        }

        public static Transform[] MeiAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher mei, string avatarType)
        {
            Transform[] transformArray;
            if ((avatarType == "Mei_C1_DH") || (avatarType == "Mei_C3_WS"))
            {
                DeleteAttachPointChildren(mei, "WeaponRightHand", false);
                transformArray = new Transform[] { AttachPartToAttachPoint(mei, "LongSword", "WeaponRightHand", weaponProtoTrans) };
                InstantiateChildrenAndAttach(weaponProtoTrans.Find("LongSword"), transformArray[0], false);
                return transformArray;
            }
            if ((avatarType == "Mei_C2_CK") || (avatarType == "Mei_C4_LD"))
            {
                DeleteAttachPointChildren(mei, "WeaponLeftHand", false);
                DeleteAttachPointChildren(mei, "WeaponRightHand", false);
                transformArray = new Transform[] { AttachPartToAttachPoint(mei, "ShortSword", "WeaponLeftHand", weaponProtoTrans), AttachPartToAttachPoint(mei, "ShortSword", "WeaponRightHand", weaponProtoTrans) };
                InstantiateChildrenAndAttach(weaponProtoTrans.Find("ShortSword"), transformArray[0], false);
                InstantiateChildrenAndAttach(weaponProtoTrans.Find("ShortSword"), transformArray[1], false);
                return transformArray;
            }
            return new Transform[0];
        }

        public static void MeiDetachHandler(ConfigWeaponAttach config, IWeaponAttacher mei, string avatarType)
        {
            if ((avatarType == "Mei_C1_DH") || (avatarType == "Mei_C3_WS"))
            {
                DetachWeaponAttachPoint(mei, "WeaponRightHand");
                DeleteAttachPointChildren(mei, "WeaponRightHand", true);
            }
            else if ((avatarType == "Mei_C2_CK") || (avatarType == "Mei_C4_LD"))
            {
                DetachWeaponAttachPoint(mei, "WeaponLeftHand");
                DetachWeaponAttachPoint(mei, "WeaponRightHand");
                DeleteAttachPointChildren(mei, "WeaponLeftHand", true);
                DeleteAttachPointChildren(mei, "WeaponRightHand", true);
            }
        }

        public static void MeiRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType)
        {
            MeiWeaponAttach attach = (MeiWeaponAttach) config.Attach;
            if (!string.IsNullOrEmpty(attach.WeaponEffectPattern))
            {
                if ((avatarType == "Mei_C1_DH") || (avatarType == "Mei_C3_WS"))
                {
                    SetTransformParentAndReset(Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(attach.WeaponEffectPattern, avatar).transform, avatar.GetAttachPoint("WeaponRightHand"));
                }
                else if ((avatarType == "Mei_C2_CK") || (avatarType == "Mei_C4_LD"))
                {
                    GameObject obj3 = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(attach.WeaponEffectPattern, avatar);
                    GameObject obj4 = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(attach.WeaponEffectPattern, avatar);
                    SetTransformParentAndReset(obj3.transform, avatar.GetAttachPoint("WeaponLeftHand"));
                    SetTransformParentAndReset(obj4.transform, avatar.GetAttachPoint("WeaponRightHand"));
                }
            }
        }

        private static void MoveAvatarAttachPoint(IWeaponAttacher avatar, string weaponAttachPointTransPath, string avatarAttachPointName, Transform protoTrans)
        {
            Transform from = protoTrans.Find(weaponAttachPointTransPath);
            Transform attachPoint = avatar.GetAttachPoint(avatarAttachPointName);
            CopyTransformLocalProperties(from, attachPoint);
        }

        private static void SetRendererLayer(Transform attachTrans, int layer)
        {
            foreach (Renderer renderer in attachTrans.GetComponentsInChildren<Renderer>())
            {
                renderer.gameObject.SetLayer(layer, false);
            }
        }

        public static void SetTransformParentAndReset(Transform child, Transform parent)
        {
            child.parent = parent;
            child.localRotation = Quaternion.identity;
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.one;
        }

        public delegate void RuntimeWeaponAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType);

        public delegate Transform[] WeaponAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher avatar, string avatarType);

        public delegate void WeaponDetachHandler(ConfigWeaponAttach config, IWeaponAttacher avatar, string avatarType);
    }
}

