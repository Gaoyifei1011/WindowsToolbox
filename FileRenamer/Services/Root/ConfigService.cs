using FileRenamer.Helpers.Root;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        public static async Task<T> ReadSettingAsync<T>(string key)
        {
            if (RuntimeHelper.IsMSIX)
            {
                if (ApplicationData.Current.LocalSettings.Values[key] is null)
                {
                    return default;
                }

                return await Task.FromResult((T)ApplicationData.Current.LocalSettings.Values[key]);
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
                        return await Task.FromResult((T)configDict[key]);
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
        public static async Task SaveSettingAsync<T>(string key, T value)
        {
            if (RuntimeHelper.IsMSIX)
            {
                ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
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
                            configDict[key] = await Task.FromResult(value);
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
