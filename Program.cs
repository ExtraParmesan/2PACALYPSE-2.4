using Photino.NET;
using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using LibVLCSharp.Shared;

namespace pacalypse
{
    class Program
    {
        private static LibVLC? _libvlc;
        private static MediaPlayer? _mediaPlayer;
        private static Media? _media; // Keep media object alive

        [STAThread]
        static void Main(string[] args)
        {
            // audio stuff
            Core.Initialize();
            _libvlc = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libvlc);

            // Create media and assign it to the player
            _media = new Media(_libvlc, "wwwroot/assets/sounds/squeak-type-beat.mp3", FromType.FromPath);
            _mediaPlayer.Media = _media;

            // endreached loop
            _mediaPlayer.EndReached += (sender, e) =>
            {
                _mediaPlayer.Play();
            };

            // Start playing
            _mediaPlayer.Play();

            // Windows
            string windowTitle = "2PACALYPSE 2.4";

            var window = new PhotinoWindow()
                .SetTitle(windowTitle)
                .SetUseOsDefaultSize(false)
                .SetSize(new Size(800, 550))
                .Center()
                .SetResizable(false)
                .Load("wwwroot/index.html");

            // event handlers
            window.RegisterWebMessageReceivedHandler((object? sender, string message) =>
            {
                var messageData = JsonSerializer.Deserialize<MessageData>(message);
                if (messageData != null)
                {
                    Console.WriteLine($"IP: {messageData.ip}, Port: {messageData.port}");
                    window.SendWebMessage($"DDoSing nigga at {messageData.ip}:{messageData.port}!");
                }
            });

            // window closing
            window.WindowClosing += (sender, e) =>
            {
                Console.WriteLine("Window closing. Stopping and disposing audio player.");
                _mediaPlayer?.Stop();
                _media?.Dispose();
                _mediaPlayer?.Dispose();
                _libvlc?.Dispose();
                return false;
            };

            window.WaitForClose();
        }
    }

    public class MessageData
    {
        public string? ip { get; set; }
        public string? port { get; set; }
    }
}

