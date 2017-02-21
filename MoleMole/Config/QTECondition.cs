namespace MoleMole.Config
{
    using System;

    public class QTECondition
    {
        public static QTECondition[] EMPTY = new QTECondition[0];
        public float QTERange;
        public QTEConditionType QTEType;
        public string[] QTEValues;
    }
}

