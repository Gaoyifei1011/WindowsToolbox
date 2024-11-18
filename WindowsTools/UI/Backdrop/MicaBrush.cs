using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WindowsTools.WindowsAPI.PInvoke.Comctl32;
using WindowsTools.WindowsAPI.PInvoke.Kernel32;
using WindowsTools.WindowsAPI.PInvoke.User32;

// 抑制 CA1822 警告
#pragma warning disable CA1822

namespace WindowsTools.UI.Backdrop
{
    /// <summary>
    /// Mica 背景色
    /// </summary>
    public sealed partial class MicaBrush : XamlCompositionBrushBase
    {
        private const int PBT_POWERSETTINGCHANGE = 0x8013;

        private bool isConnected;
        private bool isActivated = true;
        private bool isEnergySaverEnabled;
        private bool useMicaBrush;
        private IntPtr hPowerNotify;
        private SUBCLASSPROC formSubClassProc;

        private readonly float micaBaseLightTintOpacity = 0.5f;
        private readonly float micaBaseLightLuminosityOpacity = 1;
        private readonly float micaBaseDarkTintOpacity = 0.8f;
        private readonly float micaBaseDarkLuminosityOpacity = 1;
        private readonly Color micaBaseLightTintColor = Color.FromArgb(255, 243, 243, 243);
        private readonly Color micaBaseLightFallbackColor = Color.FromArgb(255, 243, 243, 243);
        private readonly Color micaBaseDarkTintColor = Color.FromArgb(255, 32, 32, 32);
        private readonly Color micaBaseDarkFallbackColor = Color.FromArgb(255, 32, 32, 32);

        private readonly float micaAltLightTintOpacity = 0.5f;
        private readonly float micaAltLightLuminosityOpacity = 1;
        private readonly float micaAltDarkTintOpacity = 0;
        private readonly float micaAltDarkLuminosityOpacity = 1;
        private readonly Color micaAltLightTintColor = Color.FromArgb(255, 218, 218, 218);
        private readonly Color micaAltLightFallbackColor = Color.FromArgb(255, 218, 218, 218);
        private readonly Color micaAltDarkTintColor = Color.FromArgb(255, 10, 10, 10);
        private readonly Color micaAltDarkFallbackColor = Color.FromArgb(255, 10, 10, 10);

        private readonly bool isInputActive;
        private readonly float lightTintOpacity;
        private readonly float lightLuminosityOpacity;
        private readonly float darkTintOpacity;
        private readonly float darkLuminosityOpacity;
        private readonly FrameworkElement backgroundElement;
        private readonly Form formRoot;
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        private Color lightTintColor = Color.FromArgb(0, 0, 0, 0);
        private Color lightFallbackColor = Color.FromArgb(0, 0, 0, 0);
        private Color darkTintColor = Color.FromArgb(0, 0, 0, 0);
        private Color darkFallbackColor = Color.FromArgb(0, 0, 0, 0);

        private Guid GUID_POWER_SAVING_STATUS = new("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");
        private readonly CompositionCapabilities compositionCapabilities = CompositionCapabilities.GetForCurrentView();

        /// <summary>
        /// 检查是否支持云母背景色
        /// </summary>
        public static bool IsSupported
        {
            get
            {
                return ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush));
            }
        }

        public MicaBrush(MicaKind micaKind, FrameworkElement frameworkElement, Form form, bool isinputActive)
        {
            if (form is null)
            {
                throw new ArgumentNullException(string.Format("参数 {0} 不可以为空值", nameof(form)));
            }

            if (frameworkElement is null)
            {
                throw new ArgumentNullException(string.Format("参数 {0} 不可以为空值", nameof(frameworkElement)));
            }

            isInputActive = isinputActive;
            backgroundElement = frameworkElement;
            formRoot = form;

            if (micaKind is MicaKind.Base)
            {
                lightTintOpacity = micaBaseLightTintOpacity;
                lightLuminosityOpacity = micaBaseLightLuminosityOpacity;
                darkTintOpacity = micaBaseDarkTintOpacity;
                darkLuminosityOpacity = micaBaseDarkLuminosityOpacity;
                lightTintColor = micaBaseLightTintColor;
                lightFallbackColor = micaBaseLightFallbackColor;
                darkTintColor = micaBaseDarkTintColor;
                darkFallbackColor = micaBaseDarkFallbackColor;
            }
            else
            {
                lightTintOpacity = micaAltLightTintOpacity;
                lightLuminosityOpacity = micaAltLightLuminosityOpacity;
                darkTintOpacity = micaAltDarkTintOpacity;
                darkLuminosityOpacity = micaAltDarkLuminosityOpacity;
                lightTintColor = micaAltLightTintColor;
                lightFallbackColor = micaAltLightFallbackColor;
                darkTintColor = micaAltDarkTintColor;
                darkFallbackColor = micaAltDarkFallbackColor;
            }
        }

        /// <summary>
        /// 在屏幕上首次使用画笔绘制元素时调用。
        /// </summary>
        protected override void OnConnected()
        {
            base.OnConnected();

            if (!isConnected)
            {
                isConnected = true;
                SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
                formRoot.Activated += OnActivated;
                formRoot.Deactivate += OnDeactivated;
                compositionCapabilities.Changed += OnCompositionCapabilitiesChanged;

                if (backgroundElement is not null)
                {
                    backgroundElement.ActualThemeChanged += OnActualThemeChanged;
                }

                formSubClassProc = new SUBCLASSPROC(OnFormSubClassProc);
                Comctl32Library.SetWindowSubclass(formRoot.Handle, formSubClassProc, 0, IntPtr.Zero);

                hPowerNotify = User32Library.RegisterPowerSettingNotification(formRoot.Handle, GUID_POWER_SAVING_STATUS, 0);
                UpdateBrush();
            }
        }

        /// <summary>
        /// 不再使用画笔绘制任何元素时调用。
        /// </summary>
        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            if (isConnected)
            {
                isConnected = false;
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
                formRoot.Activated -= OnActivated;
                formRoot.Deactivate -= OnDeactivated;
                compositionCapabilities.Changed -= OnCompositionCapabilitiesChanged;

                if (backgroundElement is not null)
                {
                    backgroundElement.ActualThemeChanged -= OnActualThemeChanged;
                }

                if (hPowerNotify != IntPtr.Zero)
                {
                    User32Library.UnregisterPowerSettingNotification(hPowerNotify);
                    hPowerNotify = IntPtr.Zero;
                }

                Comctl32Library.RemoveWindowSubclass(formRoot.Handle, formSubClassProc, 0);

                if (CompositionBrush is not null)
                {
                    CompositionBrush.Dispose();
                    CompositionBrush = null;
                }
            }
        }

        /// <summary>
        /// 在用户首选项发生更改时触发的事件
        /// </summary>
        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs args)
        {
            synchronizationContext.Post(_ =>
            {
                UpdateBrush();
            }, null);
        }

        /// <summary>
        /// 在窗口完成激活或停用时触发的事件
        /// </summary>
        private void OnActivated(object sender, EventArgs args)
        {
            isActivated = true;
            UpdateBrush();
        }

        /// <summary>
        /// 当窗体失去焦点并不再是活动窗体时触发的事件
        /// </summary>
        private void OnDeactivated(object sender, EventArgs args)
        {
            isActivated = false;
            UpdateBrush();
        }

        /// <summary>
        /// 当支持的合成功能发生更改时触发的事件
        /// </summary>
        private void OnCompositionCapabilitiesChanged(CompositionCapabilities sender, object args)
        {
            synchronizationContext.Post(_ =>
            {
                UpdateBrush();
            }, null);
        }

        /// <summary>
        /// 在 ActualTheme 属性值更改时触发的事件
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            UpdateBrush();
        }

        /// <summary>
        /// 更新应用的背景色
        /// </summary>
        private void UpdateBrush()
        {
            if (isConnected)
            {
                float tintOpacity;
                float luminosityOpacity;
                Color tintColor;
                Color fallbackColor;

                if (backgroundElement.ActualTheme is ElementTheme.Light)
                {
                    tintOpacity = lightTintOpacity;
                    luminosityOpacity = lightLuminosityOpacity;
                    tintColor = lightTintColor;
                    fallbackColor = lightFallbackColor;
                }
                else
                {
                    tintOpacity = darkTintOpacity;
                    luminosityOpacity = darkLuminosityOpacity;
                    tintColor = darkTintColor;
                    fallbackColor = darkFallbackColor;
                }

                useMicaBrush = IsSupported && IsAdvancedEffectsEnabled() && !isEnergySaverEnabled && compositionCapabilities.AreEffectsSupported() && (isInputActive || isActivated);

                Compositor compositor = Window.Current.Compositor;

                if (SystemInformation.HighContrast)
                {
                    System.Drawing.Color windowColor = System.Drawing.SystemColors.Window;
                    tintColor = Color.FromArgb(windowColor.R, windowColor.A, windowColor.G, windowColor.B);
                    useMicaBrush = false;
                }

                CompositionBrush newBrush = useMicaBrush ? BuildMicaEffectBrush(compositor, tintColor, tintOpacity, luminosityOpacity) : compositor.CreateColorBrush(fallbackColor);
                CompositionBrush oldBrush = CompositionBrush;

                if (oldBrush is null || CompositionBrush.Comment is "Crossfade")
                {
                    // 直接设置新笔刷
                    oldBrush?.Dispose();
                    CompositionBrush = newBrush;
                }
                else
                {
                    // 回退色切换时的动画颜色
                    CompositionBrush crossFadeBrush = CreateCrossFadeEffectBrush(compositor, oldBrush, newBrush);
                    ScalarKeyFrameAnimation animation = CreateCrossFadeAnimation(compositor);
                    CompositionBrush = crossFadeBrush;

                    CompositionScopedBatch crossFadeAnimationBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    crossFadeBrush.StartAnimation("CrossFade.CrossFade", animation);
                    crossFadeAnimationBatch.End();

                    crossFadeAnimationBatch.Completed += (o, a) =>
                    {
                        crossFadeBrush.Dispose();
                        oldBrush.Dispose();
                        CompositionBrush = newBrush;
                    };
                }
            }
        }

        /// <summary>
        /// 创建云母背景色
        /// </summary>
        private static CompositionEffectBrush BuildMicaEffectBrush(Compositor compositor, Color tintColor, float tintOpacity, float luminosityOpacity)
        {
            // Tint Color.
            ColorSourceEffect tintColorEffect = new()
            {
                Name = "TintColor",
                Color = tintColor
            };

            // OpacityEffect applied to Tint.
            OpacityEffect tintOpacityEffect = new()
            {
                Name = "TintOpacity",
                Opacity = tintOpacity,
                Source = tintColorEffect
            };

            // Apply Luminosity:

            // Luminosity Color.
            ColorSourceEffect luminosityColorEffect = new()
            {
                Color = tintColor
            };

            // OpacityEffect applied to Luminosity.
            OpacityEffect luminosityOpacityEffect = new()
            {
                Name = "LuminosityOpacity",
                Opacity = luminosityOpacity,
                Source = luminosityColorEffect
            };

            // Luminosity Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            BlendEffect luminosityBlendEffect = new()
            {
                Mode = BlendEffectMode.Color,
                Background = new CompositionEffectSourceParameter("BlurredWallpaperBackdrop"),
                Foreground = luminosityOpacityEffect
            };

            // Apply Tint:

            // Color Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            BlendEffect colorBlendEffect = new()
            {
                Mode = BlendEffectMode.Luminosity,
                Background = luminosityBlendEffect,
                Foreground = tintOpacityEffect
            };

            CompositionEffectBrush micaEffectBrush = compositor.CreateEffectFactory(colorBlendEffect).CreateBrush();
            micaEffectBrush.SetSourceParameter("BlurredWallpaperBackdrop", compositor.TryCreateBlurredWallpaperBackdropBrush());

            return micaEffectBrush;
        }

        /// <summary>
        /// 创建回退动画
        /// </summary>
        private CompositionEffectBrush CreateCrossFadeEffectBrush(Compositor compositor, CompositionBrush from, CompositionBrush to)
        {
            CrossFadeEffect crossFadeEffect = new()
            {
                Name = "Crossfade", // Name to reference when starting the animation.
                Source1 = new CompositionEffectSourceParameter("source1"),
                Source2 = new CompositionEffectSourceParameter("source2"),
                CrossFade = 0
            };

            List<string> corssfadeList = ["Crossfade.CrossFade"];
            CompositionEffectBrush crossFadeEffectBrush = compositor.CreateEffectFactory(crossFadeEffect, corssfadeList).CreateBrush();
            crossFadeEffectBrush.Comment = "Crossfade";

            crossFadeEffectBrush.SetSourceParameter("source1", from);
            crossFadeEffectBrush.SetSourceParameter("source2", to);
            return crossFadeEffectBrush;
        }

        /// <summary>
        /// 为回退色创建动画效果
        /// </summary>
        private ScalarKeyFrameAnimation CreateCrossFadeAnimation(Compositor compositor)
        {
            ScalarKeyFrameAnimation animation = compositor.CreateScalarKeyFrameAnimation();
            LinearEasingFunction linearEasing = compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(0.0f, 0.0f, linearEasing);
            animation.InsertKeyFrame(1.0f, 1.0f, linearEasing);
            animation.Duration = TimeSpan.FromMilliseconds(250);
            return animation;
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr OnFormSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            // 设备节电模式的状态发生更改时触发的消息
            if (Msg is WindowMessage.WM_POWERBROADCAST && (int)wParam is PBT_POWERSETTINGCHANGE)
            {
                POWERBROADCAST_SETTING setting = Marshal.PtrToStructure<POWERBROADCAST_SETTING>(lParam);

                if (setting.PowerSetting == GUID_POWER_SAVING_STATUS)
                {
                    Kernel32Library.GetSystemPowerStatus(out SYSTEM_POWER_STATUS status);
                    isEnergySaverEnabled = Convert.ToBoolean(status.SystemStatusFlag);

                    if (isConnected)
                    {
                        synchronizationContext.Post(_ =>
                        {
                            UpdateBrush();
                        }, null);
                    }
                }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 检查是否启用系统透明度效果设置
        /// </summary>
        private bool IsAdvancedEffectsEnabled()
        {
            return Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize") is RegistryKey key && key.GetValue("EnableTransparency") is object value && Convert.ToInt32(value) is 1;
        }
    }
}
