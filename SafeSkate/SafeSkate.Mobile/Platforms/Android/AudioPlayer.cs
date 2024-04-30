using Android.Media;
using Application = Microsoft.Maui.Controls.Application;

[assembly: Dependency(typeof(SafeSkate.Mobile.Platforms.Android.AudioPlayer))]
namespace SafeSkate.Mobile.Platforms.Android
{
    public class AudioPlayer : IAudioPlayer
    {
        MediaPlayer player;

        public void PlaySound()
        {
            player = MediaPlayer.Create(global::Android.App.Application.Context, Resource.Raw.sound_1);
            player.Start();
        }
    }
}
