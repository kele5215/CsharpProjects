using System.Configuration;

namespace ServerConfSection
{
    public class ServerGroup : ConfigurationElementCollection
    {
        public ServerGroup()
        {
            // 追加するときに使用する属性名をここで指定もできる
            AddElementName = "addServer";
        }

        /// <summary>
        /// 子カスタムエレメントのコレクションのタイプ
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// 子カスタムエレメントを作成
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return (ServerConfigElement)new ServerConfigElement();
        }

        /// <summary>
        /// カスタムエレメントのキー名を返す
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerConfigElement)element).ServerNm;
        }

        /// <summary>
        /// アクセッサ
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ServerConfigElement this[int index]
        {
            get { return (ServerConfigElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
    }
}
