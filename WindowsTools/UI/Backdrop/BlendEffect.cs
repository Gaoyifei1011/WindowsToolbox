using System;
using System.Runtime.InteropServices;
using Windows.Graphics.Effects;
using WindowsTools.Helpers.Backdrop;
using WindowsTools.WindowsAPI.ComTypes;

namespace WindowsTools.UI.Backdrop
{
    [Guid("81C5B77B-13F8-4CDD-AD20-C890547AC65D")]
    public sealed class BlendEffect : IGraphicsEffect, IGraphicsEffectSource, IGraphicsEffectD2D1Interop
    {
        public BlendEffectMode Mode { get; set; } = BlendEffectMode.Multiply;

        public IGraphicsEffectSource Background { get; set; }

        public IGraphicsEffectSource Foreground { get; set; }

        public string Name { get; set; } = string.Empty;

        public int GetEffectId(out Guid id)
        {
            id = typeof(BlendEffect).GUID;
            return 0;
        }

        public int GetNamedPropertyMapping(string name, out uint index, out GRAPHICS_EFFECT_PROPERTY_MAPPING mapping)
        {
            switch (name)
            {
                case nameof(Mode):
                    {
                        index = 0;
                        mapping = GRAPHICS_EFFECT_PROPERTY_MAPPING.GRAPHICS_EFFECT_PROPERTY_MAPPING_DIRECT;
                        break;
                    }
                default:
                    {
                        index = 0xFF;
                        mapping = (GRAPHICS_EFFECT_PROPERTY_MAPPING)0xFF;
                        break;
                    }
            }

            return 0;
        }

        public int GetProperty(uint index, out IntPtr source)
        {
            if (index is 0)
            {
                BackdropHelper.PropertyValueStatics.Value.CreateUInt32((uint)Mode, out IntPtr ptr);
                if (ptr != IntPtr.Zero)
                {
                    source = ptr;
                    return 0;
                }
            }

            source = IntPtr.Zero;
            return -2147483637;
        }

        public int GetPropertyCount(out uint count)
        {
            count = 1;
            return 0;
        }

        public int GetSource(uint index, out IGraphicsEffectSource source)
        {
            if (index is 0)
            {
                source = Background;
                return 0;
            }
            else if (index is 1)
            {
                source = Foreground;
                return 0;
            }
            else
            {
                source = null;
                return 2147483637;
            }
        }

        public int GetSourceCount(out uint count)
        {
            count = 2;
            return 0;
        }
    }
}
