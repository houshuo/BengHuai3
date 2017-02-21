namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoNewbieOriginTransformHelper : MonoBehaviour
    {
        public NewbieDialogContext newbieDialogContext;

        private void OnDestroy()
        {
            if ((this.newbieDialogContext != null) && this.newbieDialogContext.IsActive)
            {
                this.newbieDialogContext.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if ((this.newbieDialogContext != null) && this.newbieDialogContext.IsActive)
            {
                this.newbieDialogContext.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if ((this.newbieDialogContext != null) && !this.newbieDialogContext.IsActive)
            {
                this.newbieDialogContext.SetActive(true);
            }
        }
    }
}

