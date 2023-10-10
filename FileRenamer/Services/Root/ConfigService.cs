using FileRenamer.Extensions.DataType.Constant;
using FileRenamer.Helpers.Root;
using FileRenamer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Storage;

namespace FileRenamer.Services.Root
{
    /// <summary>
    /// 设置选项配置服务
    /// </summary>
    public static class ConfigService
    {
        public static string UnPackagedConfigFile { get; } = Path.Combine(AppContext.BaseDirectory, "settings.json");

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            if (RuntimeHelper.IsMSIX)
            {
                if (ApplicationData.Current.LocalSettings.Values[key] is null)
                {
                    return default;
                }

                return (T)ApplicationData.Current.LocalSettings.Values[key];
            }
            else
            {
                if (File.Exists(UnPackagedConfigFile))
                {
                    string settingsContent = File.ReadAllText(UnPackagedConfigFile);
                    Dictionary<string, object> configDict = JsonDeserialize(settingsContent);

                    if (configDict.ContainsKey(key) && configDict[key] is not null)
                    {
                        return (T)configDict[key];
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    return default;
                }
            }
        }

        /// <summary>
        /// 保存设置选项存储信息
        /// </summary>
        public static void SaveSetting<T>(string key, T value)
        {
            if (RuntimeHelper.IsMSIX)
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
            else
            {
                if (File.Exists(UnPackagedConfigFile))
                {
                    string settingsContent = File.ReadAllText(UnPackagedConfigFile);
                    Dictionary<string, object> configDict = JsonDeserialize(settingsContent);

                    configDict[key] = value;
                    string settingsResult = JsonSerialize(configDict);

                    File.WriteAllText(UnPackagedConfigFile, settingsResult);
                }
            }
        }

        /// <summary>
        /// 对设置存储字典进行序列化
        /// </summary>
        private static string JsonSerialize(Dictionary<string, object> configDict)
        {
            ConfigModel serializeConfigData = new ConfigModel();
            serializeConfigData.AppLanguage = configDict[ConfigKey.LanguageKey];
            serializeConfigData.AppTheme = configDict[ConfigKey.ThemeKey];
            serializeConfigData.AppBackdrop = configDict[ConfigKey.BackdropKey];
            serializeConfigData.AlwaysShowBackdrop = configDict[ConfigKey.AlwaysShowBackdropKey];
            serializeConfigData.TopMostValue = configDict[ConfigKey.TopMostKey];

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ConfigModel));
            MemoryStream serializerMemoryStream = new MemoryStream();
            serializer.WriteObject(serializerMemoryStream, serializeConfigData);
            string content = Encoding.UTF8.GetString(serializerMemoryStream.ToArray());
            serializerMemoryStream.Dispose();

            return content;
        }

        /// <summary>
        /// 对设置存储的本地字符串数据反序列化
        /// </summary>
        private static Dictionary<string, object> JsonDeserialize(string content)
        {
            Dictionary<string, object> configDict = new Dictionary<string, object>
            {
                { ConfigKey.LanguageKey, null },
                { ConfigKey.ThemeKey, null },
                { ConfigKey.BackdropKey, null },
                { ConfigKey.AlwaysShowBackdropKey, null },
                { ConfigKey.TopMostKey, null }
            };

            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ConfigModel));
                MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                ConfigModel configData = (ConfigModel)serializer.ReadObject(memoryStream);
                memoryStream.Dispose();

                configDict[ConfigKey.LanguageKey] = configData.AppLanguage;
                configDict[ConfigKey.ThemeKey] = configData.AppTheme;
                configDict[ConfigKey.BackdropKey] = configData.AppBackdrop;
                configDict[ConfigKey.AlwaysShowBackdropKey] = configData.AlwaysShowBackdrop;
                configDict[ConfigKey.TopMostKey] = configData.TopMostValue;
            }
            catch { }

            return configDict;
        }
    }
}
