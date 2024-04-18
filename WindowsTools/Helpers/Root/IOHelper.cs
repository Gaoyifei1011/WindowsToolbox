﻿using System.IO;
using System.Text;

namespace WindowsTools.Helpers.Root
{
    public static class IOHelper
    {
        /// <summary>
        /// 判断所给路径是文件夹还是目录
        /// </summary>
        public static bool IsDir(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            if ((fi.Attributes & FileAttributes.Directory) is not 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
