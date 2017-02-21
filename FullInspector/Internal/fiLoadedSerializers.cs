namespace FullInspector.Internal
{
    using FullInspector.Serializers.FullSerializer;
    using System;

    public class fiLoadedSerializers : fiILoadedSerializers
    {
        public Type[] AllLoadedSerializerProviders
        {
            get
            {
                return new Type[] { typeof(FullSerializerMetadata) };
            }
        }

        public Type DefaultSerializerProvider
        {
            get
            {
                return typeof(FullSerializerMetadata);
            }
        }
    }
}

