using System.IO;
using System.Text;

namespace FileRenamer.Helpers.Root
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

        /// <summary>
        /// 过滤不合法的文件路径字符
        /// </summary>
        public static string FilterInvalidPathChars(string path)
        {
            StringBuilder titleBuilder = new StringBuilder(path);
            foreach (char rInvalidChar in Path.GetInvalidPathChars())
            {
                titleBuilder = titleBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            }
            return titleBuilder.ToString();
        }
    }
}
