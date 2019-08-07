using System.Collections.Generic;

namespace ConvertSimpTrad
{
    public class ResourceModel
    {
        #region 私有变量

        /// <summary>
        /// 元々Resourceファイル
        /// </summary>
        private ResourceLanguage _resxOld;

        /// <summary>
        /// 切替Resourceファイル
        /// </summary>
        private ResourceLanguage _resxNew;

        /// 多選狀態下複數Resource內容
        /// </summary>
        private IList<ResourceModel> _multiple;

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public ResourceModel()
        {
            MultipleResList = new List<ResourceModel>();
        }

        public ResourceLanguage ResourceOld
        {
            get { return _resxOld; }
            set { _resxOld = value; }
        }

        public ResourceLanguage ResourceNew
        {
            get { return _resxNew; }
            set { _resxNew = value; }
        }

        /// <summary>
        /// 複數Resource內容
        /// </summary>
        public IList<ResourceModel> MultipleResList
        {
            get { return _multiple; }
            set { _multiple = value; }
        }
    }
}
