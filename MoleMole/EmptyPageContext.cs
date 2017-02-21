namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class EmptyPageContext : BasePageContext
    {
        public EmptyPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EmptyPageContext"
            };
            base.config = pattern;
        }
    }
}

