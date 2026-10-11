using Godot;

namespace PingToLaowuLaowu.Scripts;

internal static class LaowuSoundPlayer
{
    private const string ResourceDirectory = "res://PingToLaowuLaowu/LaowuSounds";
    private static string? _lastPlayed;

    public static void PlayRandom()
    {
        // Network callbacks need not be on the Godot thread.
        Callable.From(PlayOnMainThread).CallDeferred();
    }

    private static void PlayOnMainThread()
    {
        try
        {
            if (Engine.GetMainLoop() is not SceneTree tree)
                return;

            var files = FindSounds();
            // With only one file, silence is the only way to avoid consecutive repeats.
            var candidates = files.Where(path => path != _lastPlayed).ToList();
            while (candidates.Count > 0)
            {
                int index = System.Random.Shared.Next(candidates.Count);
                string path = candidates[index];
                candidates.RemoveAt(index);
                AudioStream? stream = ResourceLoader.Load<AudioStream>(path);
                if (stream == null || stream.GetLength() <= 0)
                    continue;

                var player = new AudioStreamPlayer
                {
                    Name = "LaowuPingSound",
                    Stream = stream,
                    VolumeDb = 20f,
                    Bus = AudioServer.GetBusIndex("SFX") >= 0 ? "SFX" : "Master"
                };
                player.Finished += player.QueueFree;
                tree.Root.AddChild(player);
                player.Play();
                _lastPlayed = path;
                return;
            }
        }
        catch (Exception exception)
        {
            // A missing or bad custom audio file must not break ping handling.
            Entry.Logger.Info($"Cannot play Laowu sound: {exception.Message}");
        }
    }

    private static string[] FindSounds()
    {
        // All sounds are imported resources bundled inside the mod PCK.
        return ResourceLoader.ListDirectory(ResourceDirectory)
            .Where(name => name.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            .Distinct().OrderBy(name => name, StringComparer.Ordinal)
            .Select(name => $"{ResourceDirectory}/{name}").ToArray();
    }
}
