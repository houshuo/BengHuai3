namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class MaterialGroup : IDisposable
    {
        private MaterialGroup _instancedGroup;
        public RendererMaterials[] entries;
        public string groupName;

        public MaterialGroup(string groupName)
        {
            this.groupName = groupName;
            this.entries = new RendererMaterials[0];
        }

        public MaterialGroup(string groupName, Renderer[] renderers)
        {
            this.groupName = groupName;
            this.entries = new RendererMaterials[renderers.Length];
            for (int i = 0; i < this.entries.Length; i++)
            {
                Renderer renderer = renderers[i];
                this.entries[i] = new RendererMaterials();
                if ((renderer is MeshRenderer) || (renderer is SkinnedMeshRenderer))
                {
                    this.entries[i].materials = renderers[i].materials;
                }
                else
                {
                    this.entries[i].skipped = true;
                }
            }
            List<Material> list = new List<Material>();
            for (int j = 0; j < this.entries.Length; j++)
            {
                RendererMaterials materials = this.entries[j];
                materials.colorModifiers = new MaterialColorModifier[materials.materials.Length];
                for (int k = 0; k < materials.materials.Length; k++)
                {
                    Material material = materials.materials[k];
                    bool flag = false;
                    for (int m = 0; m < list.Count; m++)
                    {
                        if (list[m].name == material.name)
                        {
                            UnityEngine.Object.Destroy(material);
                            materials.materials[k] = list[m];
                            materials.colorModifiers[k] = new MaterialColorModifier();
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        list.Add(material);
                        materials.colorModifiers[k] = new MaterialColorModifier(materials.materials[k]);
                    }
                }
            }
            this._instancedGroup = this;
        }

        public void ApplyColorModifiers()
        {
            for (int i = 0; i < this.entries.Length; i++)
            {
                MaterialColorModifier[] colorModifiers = this.entries[i].colorModifiers;
                for (int j = 0; j < colorModifiers.Length; j++)
                {
                    colorModifiers[j].ApplyAndReset();
                }
            }
        }

        public void ApplyTo(Renderer[] renderers)
        {
            for (int i = 0; i < this.entries.Length; i++)
            {
                if (!this.entries[i].skipped)
                {
                    renderers[i].materials = this.entries[i].materials;
                }
            }
        }

        private void DestroyInstancedMaterials()
        {
            for (int i = 0; i < this.entries.Length; i++)
            {
                for (int j = 0; j < this.entries[i].materials.Length; j++)
                {
                    UnityEngine.Object.Destroy(this.entries[i].materials[j]);
                }
            }
        }

        public void Dispose()
        {
            if (this._instancedGroup != null)
            {
                this._instancedGroup.DestroyInstancedMaterials();
            }
        }

        public Material[] GetAllMaterials()
        {
            int num = 0;
            for (int i = 0; i < this.entries.Length; i++)
            {
                num += this.entries[i].materials.Length;
            }
            Material[] array = new Material[num];
            int index = 0;
            for (int j = 0; j < this.entries.Length; j++)
            {
                this.entries[j].materials.CopyTo(array, index);
                index += this.entries[j].materials.Length;
            }
            return array;
        }

        public MaterialGroup GetInstancedMaterialGroup()
        {
            if (this._instancedGroup == null)
            {
                this._instancedGroup = new MaterialGroup(string.Format("{0} (Instanced)", this.groupName));
                this._instancedGroup.entries = new RendererMaterials[this.entries.Length];
                int num = 0;
                for (int i = 0; i < this.entries.Length; i++)
                {
                    RendererMaterials materials = this.entries[i];
                    Material[] materialArray = new Material[materials.materials.Length];
                    MaterialColorModifier[] modifierArray = new MaterialColorModifier[materials.materials.Length];
                    for (int j = 0; j < materialArray.Length; j++)
                    {
                        materialArray[j] = new Material(materials.materials[j]);
                        materialArray[j].name = string.Format("{0} #{1}", materials.materials[j].name, num++);
                        modifierArray[j] = new MaterialColorModifier(materialArray[j]);
                    }
                    this._instancedGroup.entries[i] = new RendererMaterials();
                    this._instancedGroup.entries[i].materials = materialArray;
                    this._instancedGroup.entries[i].colorModifiers = modifierArray;
                    this._instancedGroup.entries[i].skipped = materials.skipped;
                }
            }
            return this._instancedGroup;
        }

        [Serializable]
        public class RendererMaterials
        {
            public MaterialColorModifier[] colorModifiers = new MaterialColorModifier[0];
            public static Material[] EMPTY_MATERIALS = new Material[0];
            public Material[] materials = EMPTY_MATERIALS;
            [NonSerialized]
            public bool skipped;
        }
    }
}

