using PowerToolbox.Helpers.Backdrop;
using PowerToolbox.WindowsAPI.ComTypes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Graphics.Effects;

namespace PowerToolbox.Views.Backdrop
{
    [Guid("48FC9F51-F6AC-48F1-8B58-3B28AC46F76D")]
    public sealed class CompositeEffect : IGraphicsEffect, IGraphicsEffectSource, IGraphicsEffectD2D1Interop
    {
        public CanvasComposite Mode { get; set; } = CanvasComposite.SourceOver;

        public string Name { get; set; } = string.Empty;

        public List<IGraphicsEffectSource> Sources { get; set; } = [];

        public int GetEffectId(out Guid id)
        {
            id = typeof(CompositeEffect).GUID;
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
                BackdropHelper.PropertyValueStatics.CreateUInt32((uint)Mode, out IntPtr ptr);

                if (!ptr.Equals(IntPtr.Zero))
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
            if (index < Sources.Count)
            {
                source = Sources[(int)index];
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
            count = (uint)Sources.Count;
            return 0;
        }
    }
}
