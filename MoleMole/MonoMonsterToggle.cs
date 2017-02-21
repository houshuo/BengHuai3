namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoMonsterToggle : MonoBehaviour
    {
        public ToggleColumn column;
        public MonoDebugPanel debugPanel;
        public string toggleValue;

        public void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                if (this.column == ToggleColumn.CATEGORY)
                {
                    this.debugPanel.OnMonsterCategoryToggleValueChanged(base.gameObject.GetComponent<Toggle>());
                }
                else if (this.column == ToggleColumn.NAME)
                {
                    this.debugPanel.OnMonsterNameToggleValueChanged(base.gameObject.GetComponent<Toggle>());
                }
                else if (this.column == ToggleColumn.TYPE)
                {
                    this.debugPanel.OnMonsterTypeToggleValueChanged(base.gameObject.GetComponent<Toggle>());
                }
            }
        }

        public void SetMonsterToggleValue(MonoDebugPanel debugPanel, string value, ToggleColumn column)
        {
            this.debugPanel = debugPanel;
            this.toggleValue = value;
            this.column = column;
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        public enum ToggleColumn
        {
            CATEGORY,
            NAME,
            TYPE
        }
    }
}

