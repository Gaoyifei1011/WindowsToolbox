namespace FileRenamer.Models
{
    /// <summary>
    /// 键值对（使用列表模仿字典（因为字典类型无法绑定））数据模型
    /// </summary>
    public class ReferenceKeyValuePairModel
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
