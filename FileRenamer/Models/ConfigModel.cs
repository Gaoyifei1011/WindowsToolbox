using FileRenamer.Extensions.DataType.Constant;
using System.Runtime.Serialization;

namespace FileRenamer.Models
{
    /// <summary>
    /// 应用设置序列化和反序列化数据模型
    /// </summary>
    [DataContract]
    public class ConfigModel
    {
        [DataMember(Name = ConfigKey.LanguageKey)]
        public object AppLanguage { get; set; }

        [DataMember(Name = ConfigKey.ThemeKey)]
        public object AppTheme { get; set; }

        [DataMember(Name = ConfigKey.BackdropKey)]
        public object AppBackdrop { get; set; }

        [DataMember(Name = ConfigKey.AlwaysShowBackdropKey)]
        public object AlwaysShowBackdrop { get; set; }

        [DataMember(Name = ConfigKey.TopMostKey)]
        public object TopMostValue { get; set; }
    }
}
