<div align=center>
<img src="https://github.com/Gaoyifei1011/FileRenamer/assets/49179966/5f1dce26-8767-439e-8211-98cb0182acdf" width="140" height="140"/>
</div>

# <p align="center">欢迎访问 文件重命名工具</p>

### 语言选择（Language selection）

> * [简体中文](https://github.com/Gaoyifei1011/FileRenamer/blob/main/Description/README_ZH-CN.md)&emsp;
> * [English](https://github.com/Gaoyifei1011/FileRenamer/blob/main/Description/README_EN-US.md)&emsp;

------

### 应用简介

一个简单的可以帮助用户快速重命名大量文件的工具，提供了简单丰富的重命名选项。

------

### 该应用的基础功能

> * 批量修改文件的名称
> * 批量修改文件的扩展名称
> * 批量修改文件的大写小写名称
> * 批量修改文件的属性

------

### 应用截图

#### <p align="center">文件名称页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/FileRenamer/assets/49179966/34154185-1212-469c-88f5-64aa7df63e8d">
</div>

#### <p align="center">扩展名称页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/FileRenamer/assets/49179966/d90e23db-1ee2-4160-a9cc-c0cd7ad508a2">
</div>

#### <p align="center">大写小写页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/FileRenamer/assets/49179966/3af8df88-4fb1-4b68-8bce-d39795de39df">
</div>

#### <p align="center">文件属性页面</p>
<div align="center">
<img src="https://github.com/Gaoyifei1011/FileRenamer/assets/49179966/fb740e5d-6fce-4f5c-a18f-f94f982bd4e9">
</div>

------

### 项目引用（按英文首字母排序）

> * [Mile.Xaml](https://github.com/ProjectMile/Mile.Xaml)&emsp;
> * [Microsoft.UI.Xaml](https://github.com/microsoft/microsoft-ui-xaml)&emsp;
> * [Microsoft.WindowsAppSDK](https://github.com/microsoft/windowsappsdk)&emsp;
> * [Microsoft.Windows.CppWinRT](https://github.com/Microsoft/cppwinrt)&emsp;

[学习过程中参考或使用的代码](https://github.com/Gaoyifei1011/FileRenamer/blob/main/Description/StudyReferenceCode.md)&emsp;

------

### 下载与安装注意事项

> * 该程序使用的是Mile.Xaml（Xaml Islands）构建的，建议您的系统版本为Windows 11（代号 21H2 / 内部版本号 22000）或更高版本，最低版本为Windows 10（代号2004 / 内部版本号19041）或更高版本。
> * 如果您的系统低于Windows 11（代号 22H2 / 内部版本号 22621），应用功能存在一些限制：
    不支持设置背景色
> * [Release](https://github.com/Gaoyifei1011/FileRenamer/releases)页面的二进制安装文件已经打包成压缩包。请解压压缩包并使用Powershell管理员模式（必要情况下）运行install.ps1文件方可实现快速安装。
> * 自行下载项目源代码并编译。（请仔细阅读下面的项目编译步骤）

------

### 项目编译步骤和应用本地化

#### <p align="center">必须安装的工具</p>

> * [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/) 
> * .NET桌面开发（打开解决方案后根据Visual Studio的工具提示内容进行安装，.NET Framework SDK 版本 4.8.1）

#### <p align="center">编译步骤</p>

打包版本
> * 克隆项目并下载代码到本地
> * 使用Visual Studio 2022打开FileRenamer.sln文件，如果解决方案提示部分工具没有安装，请完成安装工具步骤后再次打开该解决方案。
> * 还原项目的Nuget包。
> * 还原完成后，右键FileRenamerPackage，将该项目设为启动项目，并点击部署。
> * 部署完成后打开“开始”菜单即可运行应用。

未打包版本
> * 克隆项目并下载代码到本地
> * 使用Visual Studio 2022打开FileRenamer.sln文件，如果解决方案提示部分工具没有安装，请完成安装工具步骤后再次打开该解决方案。
> * 还原项目的Nuget包。
> * 还原完成后，右键FileRenamerPackage，并点击生成（不要部署）。
> * FileRenamerPackage项目生成完成后，右键FileRenamer，将该项目设为启动项目，并点击生成。
  * FileRenamer项目生成完成后，打开本地生成的目录（相对路径：FileRenamer\bin\x64\Debug(Release)\net481\win10-x64）,并同时打开FileRenamerPackage项目生成的目录（相对路径：FileRenamerPackage\bin\x64\Debug(Release)）,将该目录下的resources.pri文件拷贝到FileRenamer项目生成的目录下。
  * 拷贝完成后即可运行未打包的FileRenamer.exe程序。

#### <p align="center">应用本地化</p>
##### 项目在最初仅提供简体中文和英文两种语言格式，如果您想将应用翻译到您熟悉的语言或纠正已完成翻译的内容中存在的错误，请参考下面的步骤

> * 在Description文件夹中寻找Readme模板文件，例如英文版的是README_EN-US.md文件，将其重命名为README_(对应的语言).md文件。
> * 打开重命名后的文件，翻译所有的语句后并保存。翻译完成后请您认真检查一下。
> * 打开项目主页面的README.md，在最上方的“语言选择”中添加您对应的语言。例如“英文”，注意该文字附带超链接。
> * README_(对应的语言).md文件中添加的语言截图替换为您熟悉的语言的应用截图。
> * 完成上面所述的翻译步骤，确保所有步骤能够顺利运行。
> * 打开FileRenamerPackage打包项目，找到Package.appxmanifest文件，右键该文件，点击查看代码，找到Resources标签，根据模板添加相对应的语言，例如“<Resource Language="EN-US"/>”。
> * 打开FileRenamer项目的Strings文件夹，并创建您使用的语言，比如（English(United States)）文件名称为*.en-us.resx，具体可以参考表示语言(文化)代码与国家地区对照表）。
> * 打开子文件夹下的resx文件，对每一个名称进行翻译。
> * 编译运行代码并测试您的语言，应用在初次打开的时候如果没有您使用的语言默认显示English(United States)，需要在设置中动态调整。
> * 完成上述步骤后创建PR，然后将修改的内容提交到本项目，等待合并即可。

------

### 感谢（按英文首字母排序）

> * [AndromedaMelody](https://github.com/AndromedaMelody)&emsp;
> * [MouriNaruto](https://github.com/MouriNaruto)&emsp;

------

### 其他内容

> * 该项目自2023年3月20日起，到2023年7月29日结束，共历时4个月零9天。
> * 该项目是基于MIT协议许可的开源项目，您可以修改、分发该项目或将副本与新副本合并。如果您使用了该项目，请勿用于非法用途，本开发者不会承担任何责任。
> * [工具原型](https://github.com/Gaoyifei1011/FileRenamer/blob/main/Description/RawApplicationDescription.md)&emsp;

