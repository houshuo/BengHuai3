namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Text;

    public class AnimatorEventTest : AnimatorEvent
    {
        public float[] FloatArray;
        public float FloatValue;
        public int[] IntArray;
        public int IntValue;
        public string[] StringArray;
        public string StringValue;

        public string GetArrayText(Array array)
        {
            StringBuilder builder = new StringBuilder("[");
            if (array != null)
            {
                int index = 0;
                int length = array.Length;
                while (index < length)
                {
                    if (index != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(array.GetValue(index).ToString());
                    index++;
                }
            }
            builder.Append("]");
            return builder.ToString();
        }

        public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
        {
        }
    }
}

