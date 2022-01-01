// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;

namespace sbtw.Editor.Overlays.Settings
{
    public class InputSection : SettingsSection
    {
        public override LocalisableString Header => @"Input";

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.MousePointer };

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            if (host.AvailableInputHandlers.FirstOrDefault(h => h is MouseHandler) is not MouseHandler mouseHandler)
                return;

            Children = new Drawable[]
            {
                new MouseSettings(mouseHandler),
            };
        }

        private class MouseSettings : SettingsSubsection
        {
            protected override LocalisableString Header => "Mouse";

            private readonly MouseHandler mouseHandler;

            private Bindable<bool> relativeMode;
            private Bindable<double> localSensitivity;
            private Bindable<double> handlerSensitivity;

            public MouseSettings(MouseHandler mouseHandler)
            {
                this.mouseHandler = mouseHandler;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                handlerSensitivity = mouseHandler.Sensitivity.GetBoundCopy();
                localSensitivity = handlerSensitivity.GetUnboundCopy();
                relativeMode = mouseHandler.UseRelativeMode.GetBoundCopy();

                Children = new Drawable[]
                {
                    new SettingsCheckbox
                    {
                        LabelText = "High precision mouse",
                        Current = relativeMode
                    },
                    new SensitivitySetting
                    {
                        LabelText = "Cursor sensitivity",
                        Current = localSensitivity
                    },
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                relativeMode.BindValueChanged(relative => localSensitivity.Disabled = !relative.NewValue, true);

                handlerSensitivity.BindValueChanged(val =>
                {
                    var disabled = localSensitivity.Disabled;

                    localSensitivity.Disabled = false;
                    localSensitivity.Value = val.NewValue;
                    localSensitivity.Disabled = disabled;
                }, true);

                localSensitivity.BindValueChanged(val => handlerSensitivity.Value = val.NewValue);
            }

            private class SensitivitySetting : SettingsSlider<double, SensitivitySlider>
            {
                public SensitivitySetting()
                {
                    KeyboardStep = 0.01f;
                    TransferValueOnCommit = true;
                }
            }

            private class SensitivitySlider : OsuSliderBar<double>
            {
                public override LocalisableString TooltipText => Current.Disabled ? "enable high precision mouse to adjust sensitivity" : $"{base.TooltipText}x";
            }
        }
    }
}
