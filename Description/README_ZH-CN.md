<div align=center>
<img src="https://github.com/Gaoyifei1011/WindowsToolbox/assets/49179966/8d58b720-9e84-468a-a680-ac90e4f78ae4" width="140" height="140"/>
</div>

# <p align="center">欢迎访问 Windows 工具箱</p>

### 应用简介

一个集成了多个小工具的工具箱。

------

### 该应用的基础功能

> * 批量修改文件的名称
> * 批量修改文件的扩展名称
> * 批量修改文件的大写小写名称
> * 批量修改文件的属性
> * 批量删除已签名文件的证书
> * 提取文件图标和包资源索引内容
> * Windows 系统性能评估

------

### 应用截图

#### <p align="center">摸鱼页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/WindowsToolbox/assets/49179966/ee8f736e-fbf5-4fc3-9050-4fb0d5a08b74">
</div>

#### <p align="center">文件工具页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/WindowsToolbox/assets/49179966/95130de1-a57b-4c0d-b851-a21d2cd3b740">
</div>

#### <p align="center">图标提取页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/WindowsToolbox/assets/49179966/f9b3679d-e970-4f1e-8105-d26ebf4295b9">
</div>

#### <p align="center">包资源索引提取页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/WindowsToolbox/assets/49179966/95130de1-a57b-4c0d-b851-a21d2cd3b740">
</div>

#### <p align="center">系统评估页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/WindowsToolbox/assets/49179966/279ababe-5486-4311-9aa1-76c2be4aa8a5">
</div>

------

### 项目开发进展

| 项目内容                         | 开发进展                                                           |
| -------------------------------- | -------------------------------------------------------------------|
| 摸鱼                             | 已完成                                                             |
| 批量修改文件的名称               | 已完成                                                             |
| 批量修改文件的扩展名称           | 已完成                                                             |
| 批量修改文件的大写小写名称       | 已完成                                                             |
| 批量修改文件的属性               | 已完成                                                             |
| 删除已签名文件的证书             | 已完成                                                             |
| 文件解锁                         | 已完成                                                             |
| 下载管理（传递优化）             | 已完成                                                             |
| 条形码二维码生成解析             | 已完成                                                             |
| 提取文件图标                     | 已完成                                                             |
| 提取包资源文件索引内容           | 已完成                                                             |
| 颜色选择器                       | 未完成                                                             |
| 自定义右键菜单                   | 已完成                                                             |
| 系统信息                         | 未完成                                                             |
| 驱动管理                         | 未完成                                                             |
| 更新管理                         | 未完成                                                             |
| 网络回环管理                     | 已完成                                                             |
| Windows 系统评估                 | 已完成                                                             |

------

### 项目引用（按英文首字母排序）

> * [Microsoft.UI.Xaml](https://github.com/microsoft/microsoft-ui-xaml)&emsp;
> * [Microsoft.Windows.SDK.BuildTools](https://aka.ms/WinSDKProjectURL)&emsp;
> * [Microsoft.Windows.SDK.Contracts](https://aka.ms/WinSDKProjectURL)&emsp;
> * [Microsoft.WindowsAppSDK](https://github.com/microsoft/windowsappsdk)&emsp;
> * [Mile.Xaml](https://github.com/ProjectMile/Mile.Xaml)&emsp;
> * [System.Numerics.Vectors](https://dot.net)&emsp;
> * [System.Private.Uri](https://dot.net)&emsp;
> * [ZXing.Net](https://github.com/micjahn/ZXing.Net)&emsp;
	
[学习过程中参考或使用的代码](https://github.com/Gaoyifei1011/WindowsToolbox/blob/main/Description/StudyReferenceCode.md)&emsp;

------

### 下载与安装注意事项

> * 该程序使用的是Mile.Xaml（Xaml Islands）构建的，建议您的系统版本为Windows 11（代号 22H2 / 内部版本号 22621）或更高版本，最低版本为Windows 11（代号 2004 / 内部版本号 19041）或更高版本。
> * [Release](https://github.com/Gaoyifei1011/WindowsToolbox/releases)页面的二进制安装文件已经打包成压缩包。请解压压缩包并使用Powershell管理员模式（必要情况下）运行install.ps1文件方可实现快速安装。
> * 自行下载项目源代码并编译。（请仔细阅读下面的项目编译步骤）

------

### 项目编译步骤和应用本地化

#### <p align="center">必须安装的工具</p>

> * [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/) 
> * .NET桌面开发（打开解决方案后根据Visual Studio的工具提示内容进行安装，.NET Framework SDK 版本 4.8.1）

#### <p align="center">编译步骤</p>

打包版本
> * 克隆项目并下载代码到本地
> * 使用Visual Studio 2022打开WindowsToolbox.sln文件，如果解决方案提示部分工具没有安装，请完成安装工具步骤后再次打开该解决方案。
> * 还原项目的Nuget包。
> * 还原完成后，右键WindowsToolboxPackage，将该项目设为启动项目，并点击部署。
> * 部署完成后打开“开始”菜单即可运行应用。

#### <p align="center">应用本地化</p>
##### 项目在最初仅提供简体中文和英文两种语言格式，如果您想将应用翻译到您熟悉的语言或纠正已完成翻译的内容中存在的错误，请参考下面的步骤。

> * 在Description文件夹中寻找Readme模板文件，例如英文版的是README_EN-US.md文件，将其重命名为README_(对应的语言).md文件。
> * 打开重命名后的文件，翻译所有的语句后并保存。翻译完成后请您认真检查一下。
> * 打开项目主页面的README.md，在最上方的“语言选择”中添加您对应的语言。例如“英文”，注意该文字附带超链接。
> * README_(对应的语言).md文件中添加的语言截图替换为您熟悉的语言的应用截图。
> * 完成上面所述的翻译步骤，确保所有步骤能够顺利运行。
> * 打开WindowsToolboxPackage打包项目，找到Package.appxmanifest文件，右键该文件，点击查看代码，找到Resources标签，根据模板添加相对应的语言，例如“<Resource Language="EN-US"/>”。
> * 打开WindowsToolbox项目的Strings文件夹，并创建您使用的语言，比如（English(United States)）文件名称为*.en-us.resx，具体可以参考表示语言(文化)代码与国家地区对照表）。
> * 打开子文件夹下的resx文件，对每一个名称进行翻译。
> * 编译运行代码并测试您的语言，应用在初次打开的时候如果没有您使用的语言默认显示English(United States)，需要在设置中动态调整。
> * 完成上述步骤后创建PR，然后将修改的内容提交到本项目，等待合并即可。

------

### 感谢（按英文首字母排序）

> * [AndromedaMelody](https://github.com/AndromedaMelody)&emsp;
> * [cnbluefire](https://github.com/cnbluefire)&emsp;
> * [MicaApps](https://github.com/MicaApps)&emsp;
> * [MouriNaruto](https://github.com/MouriNaruto)&emsp;

------

### 其他内容

> * 该项目是基于MIT协议许可的开源项目，您可以修改、分发该项目或将副本与新副本合并。如果您使用了该项目，请勿用于非法用途，本开发者不会承担任何责任。
> * [工具原型](https://github.com/Gaoyifei1011/WindowsToolbox/blob/main/Description/RawApplicationDescription.md)&emsp;
