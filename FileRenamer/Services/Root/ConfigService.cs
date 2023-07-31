using FileRenamer.Helpers.Root;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                    Dictionary<string, object> configDict;
                    try
                    {
                        configDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsContent);
                    }
                    catch
                    {
                        configDict = new Dictionary<string, object>();
                    }

                    if (configDict is not null && configDict.ContainsKey(key))
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
                    Dictionary<string, object> configDict;
                    try
                    {
                        configDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsContent);
                    }
                    catch
                    {
                        configDict = new Dictionary<string, object>();
                    }

                    if (configDict is not null)
                    {
                        if (configDict.ContainsKey(key))
                        {
                            configDict[key] = value;
                        }
                        else
                        {
                            configDict.Add(key, value);
                        }
                    }
                    else
                    {
                        configDict = new Dictionary<string, object>
                        {
                            { key, value }
                        };
                    }

                    string settingsResult = JsonConvert.SerializeObject(configDict);
                    File.WriteAllText(UnPackagedConfigFile, settingsResult);
                }
            }
        }
    }
}
