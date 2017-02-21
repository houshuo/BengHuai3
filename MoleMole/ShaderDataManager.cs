namespace MoleMole
{
    using System;

    public class ShaderDataManager
    {
        private MonoBuffShader_Base[] ShaderDataList = new MonoBuffShader_Base[Enum.GetValues(typeof(E_ShaderData)).Length];

        private ShaderDataManager()
        {
        }

        public T GetBuffShaderData<T>(E_ShaderData buff) where T: MonoBuffShader_Base
        {
            if (this.ShaderDataList[(int) buff] == null)
            {
                this.ShaderDataList[(int) buff] = Singleton<AuxObjectManager>.Instance.LoadAuxObjectProto(buff.ToString()).GetComponent<MonoBuffShader_Base>();
            }
            return (T) this.ShaderDataList[(int) buff];
        }

        public void InitAtAwake()
        {
            E_ShaderData[] dataArray = new E_ShaderData[] { E_ShaderData.ColorBias, E_ShaderData.AvatarHelper };
            foreach (E_ShaderData data in dataArray)
            {
                this.ShaderDataList[(int) data] = Singleton<AuxObjectManager>.Instance.LoadAuxObjectProto(data.ToString()).GetComponent<MonoBuffShader_Base>();
            }
        }
    }
}

