using SoundMixerSoftware.Framework.Buttons;
using SoundMixerSoftware.Framework.Profile;

namespace SoundMixerSoftware.Framework.Utils
{
    public class ButtonUtil
    {
        public static void AddButton(int index, IButtonFunction function)
        {
            var buttonStruct = ButtonHandler.AddFunction(index, function);
            ProfileHandler.SelectedProfile.Buttons[index].Functions.Add(buttonStruct);
            ProfileHandler.SaveSelectedProfile();
        }
    }
}