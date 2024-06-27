using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension.Commands
{
    /// <summary>
    /// 命令栏的命令枚举接口实现类
    /// </summary>
    [GeneratedComClass]
    public partial class EnumExplorerCommand(SubExplorerCommand[] explorerCommands) : IEnumExplorerCommand
    {
        private readonly SubExplorerCommand[] subExplorerCommands = explorerCommands is null ? ([]) : explorerCommands;
        private uint index;

        /// <summary>
        /// 目前尚未实现。
        /// </summary>
        public int Clone(out IEnumExplorerCommand ppenum)
        {
            EnumExplorerCommand enumExplorerCommand = new(subExplorerCommands)
            {
                index = index
            };
            ppenum = enumExplorerCommand;
            return 0;
        }

        /// <summary>
        /// 检索指定数量的直接跟随当前元素的元素。
        /// </summary>
        public int Next(uint celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0)] IExplorerCommand[] pUICommand, out uint pceltFetched)
        {
            pceltFetched = 0;

            if (index <= subExplorerCommands.Length)
            {
                uint uIndex = 0;

                while (uIndex < celt && index < subExplorerCommands.Length)
                {
                    pUICommand[uIndex] = subExplorerCommands[index];
                    uIndex++;
                    index++;
                }

                pceltFetched = uIndex;

                if (uIndex == celt)
                {
                    return 0;
                }
            }

            return 1;
        }

        /// <summary>
        /// 将枚举重置为 0。
        /// </summary>
        public int Reset()
        {
            index = 0;
            return 0;
        }

        /// <summary>
        /// 目前尚未实现。
        /// </summary>
        public int Skip(uint celt)
        {
            index += celt;
            if (index > subExplorerCommands.Length)
            {
                index = (uint)subExplorerCommands.Length;
                return 1;
            }
            return 0;
        }
    }
}
