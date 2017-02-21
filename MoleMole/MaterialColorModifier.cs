namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MaterialColorModifier
    {
        private Color currentColor;
        private Material material;
        private List<Multiplier> multiplierList;
        private Color originalColor;
        private int propId;

        public MaterialColorModifier()
        {
            this.propId = -1;
        }

        public MaterialColorModifier(Material material)
        {
            this.material = material;
            this.propId = -1;
            if (material.HasProperty("_Color"))
            {
                this.propId = Shader.PropertyToID("_Color");
            }
            else if (material.HasProperty("_MainColor"))
            {
                this.propId = Shader.PropertyToID("_MainColor");
            }
            if (this.IsValid())
            {
                this.originalColor = material.GetColor(this.propId);
                this.multiplierList = new List<Multiplier>();
            }
        }

        public Multiplier AddMultiplier()
        {
            if (!this.IsValid())
            {
                return null;
            }
            Multiplier item = new Multiplier();
            this.multiplierList.Add(item);
            return item;
        }

        public void ApplyAndReset()
        {
            if (this.IsValid())
            {
                for (int i = 0; i < this.multiplierList.Count; i++)
                {
                    this.currentColor *= this.multiplierList[i].mulColor;
                }
                this.material.SetColor(this.propId, this.currentColor);
                this.currentColor = this.originalColor;
            }
        }

        public bool IsValid()
        {
            return (this.propId != -1);
        }

        public void Multiply(Color mulColor)
        {
            if (this.IsValid())
            {
                this.currentColor *= mulColor;
            }
        }

        public void RemoveMultiplier(Multiplier multiplier)
        {
            if (this.IsValid())
            {
                this.multiplierList.Remove(multiplier);
            }
        }

        public class Multiplier
        {
            public Color mulColor = Color.white;
        }
    }
}

