using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WindowsToolsShellExtension.WindowsAPI.ComTypes;

namespace WindowsToolsShellExtension.Commands
{
    /// <summary>
    /// 命令栏的命令枚举接口实现类
    /// </summary>
    [GeneratedComClass]
    public partial class EnumExplorerCommand : IEnumExplorerCommand
    {
        private SubExplorerCommand[] subExplorerCommands;
        private uint index;

        public EnumExplorerCommand(SubExplorerCommand[] explorerCommands)
        {
            if (explorerCommands is null)
            {
                subExplorerCommands = Array.Empty<SubExplorerCommand>();
            }
            else
            {
                subExplorerCommands = explorerCommands;
            }
        }

        /// <summary>
        /// 目前尚未实现。
        /// </summary>
        public int Clone(out IEnumExplorerCommand ppenum)
        {
            EnumExplorerCommand enumExplorerCommand = new EnumExplorerCommand(subExplorerCommands);
            enumExplorerCommand.index = index;
            ppenum = enumExplorerCommand;
            return 0;
        }

        /// <summary>
        /// 检索指定数量的直接跟随当前元素的元素。
        /// </summary>
        public int Next(uint celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0)] out IExplorerCommand[] pUICommand, out uint pceltFetched)
        {
            pceltFetched = Math.Min(celt, (uint)subExplorerCommands.Length - index);

            if (pceltFetched is 0)
            {
                pUICommand = Array.Empty<SubExplorerCommand>();
            }
            else
            {
                pUICommand = new SubExplorerCommand[pceltFetched];

                for (uint i = 0; i < pceltFetched; i++)
                {
                    pUICommand[i] = subExplorerCommands[index + i];
                }

                index += pceltFetched;
            }

            return index == pUICommand.Length ? 1 : 0;
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
            index = (uint)Math.Min(index + celt, subExplorerCommands.Length);
            return 0;
        }
    }
}
