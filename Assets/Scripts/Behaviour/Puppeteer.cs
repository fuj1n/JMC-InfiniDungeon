using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Puppeteer : MonoBehaviour
{
    public TextAsset skeleton;

    private Dictionary<string, Transform> bones = new Dictionary<string, Transform>();
    private Dictionary<byte[], Clip> cache = new Dictionary<byte[], Clip>();

    private Clip playing;

    private bool moving = false;
    private bool paused = false;

    private bool resetting = false;
    private float resetProgress = 0F;

    private int frame;
    private int lastKey;
    private int nextKey;
    private int keyId;

    private Dictionary<Transform, Movement> lastPose = new Dictionary<Transform, Movement>();
    private Dictionary<Transform, Movement> targetPose = new Dictionary<Transform, Movement>();

    private void Awake()
    {
        foreach (string s in skeleton.text.Split('\n'))
            try
            {
                bones.Add(s.Trim(), transform.FindRecursively(s.Trim()).transform);
            }
            catch (Exception)
            {
                Debug.LogWarning("Failed to add transform: " + s);
            }
    }

    public void Play(TextAsset clip)
    {
        playing = LoadClip(clip);
        frame = 0;
        keyId = 0;
        lastKey = 0;
        if (playing != null)
            nextKey = playing.frames[keyId].frame;
        moving = true;
        resetting = false;
        paused = false;
        ResetFrame();
    }

    public void Stop()
    {
        Play(null);
        resetting = true;
    }

    public void Pause(bool paused)
    {
        this.paused = paused;
    }

    public bool IsPlaying()
    {
        return moving;
    }

    private void ResetFrame()
    {
        lastPose.Clear();

        foreach (Transform bone in bones.Values)
            lastPose.Add(bone, new Movement() { position = bone.localPosition, rotation = bone.localRotation.eulerAngles, scale = bone.localScale });

        if (playing != null && keyId < playing.frames.Length)
            targetPose = playing.frames[keyId].movement;
    }

    private void LateUpdate()
    {
        if (resetting)
        {
            resetProgress += Time.deltaTime * 2.5F;

            foreach (var pose in lastPose)
            {
                Transform t = pose.Key;
                Movement source = lastPose[t];

                t.localPosition = Vector3.Lerp(source.position, t.localPosition, resetProgress);
                t.localRotation = Quaternion.Lerp(Quaternion.Euler(source.rotation), t.localRotation, resetProgress);
                t.localScale = Vector3.Lerp(source.scale, t.localScale, resetProgress);
            }

            return;
        }

        resetProgress = 0F;

        if (playing == null)
            return;

        float progress = (frame - lastKey) / (float)(nextKey - lastKey);

        foreach (var pose in targetPose)
        {
            Transform t = pose.Key;
            Movement source = lastPose[t];
            Movement target = pose.Value;

            t.localPosition = Vector3.Lerp(source.position, target.position, progress);
            t.localRotation = Quaternion.Lerp(Quaternion.Euler(source.rotation), Quaternion.Euler(target.rotation), progress);
            t.localScale = Vector3.Lerp(source.scale, target.scale, progress);
        }

        if (paused || !moving)
            return;

        if (frame == nextKey)
        {
            keyId++;
            lastKey = nextKey;

            if (keyId < playing.frames.Length)
                nextKey = playing.frames[keyId].frame;
            else
                moving = false;

            ResetFrame();
        }

        frame++;
    }

    private Clip LoadClip(TextAsset clip)
    {
        if (clip == null)
            return null;

        if (cache.ContainsKey(clip.bytes))
            return cache[clip.bytes];

        Clip c = new Clip();
        FileData fd = JsonConvert.DeserializeObject<FileData>(clip.text.Trim());
        List<Frame> frames = new List<Frame>();

        int highestFrame = int.MinValue;

        foreach (var f in fd.frames)
        {
            if (frames.Find(fr => f.Key == fr.frame) != default(Frame))
            {
                Debug.LogWarning("Frame " + f.Key + " is specified more than once");
                continue;
            }

            Frame frame = new Frame();
            frame.frame = f.Key;

            foreach (var p in f.Value)
            {
                if (!bones.ContainsKey(p.Key))
                {
                    Debug.LogWarning("Failed to find bone " + p.Key + " whilst loading " + clip.name);

                    foreach (var dbg in bones)
                        Debug.Log(dbg.Key);

                    continue;
                }

                if (frame.movement.ContainsKey(bones[p.Key]))
                {
                    Debug.LogWarning("Bone " + p.Key + " specified more than once in frame " + frame.frame + " of " + clip.name);
                    continue;
                }

                frame.movement[bones[p.Key]] = p.Value;
            }

            frames.Add(frame);
            if (f.Key > highestFrame)
                highestFrame = f.Key;
        }

        c.length = highestFrame;
        c.frames = frames.OrderBy(k => k.frame).ToArray();

        cache.Add(clip.bytes, c);

        return c;
    }

    private class Clip
    {
        public int length;

        public Frame[] frames;
    }

    private class Frame
    {
        public int frame;
        public Dictionary<Transform, Movement> movement = new Dictionary<Transform, Movement>();
    }

    private class Movement
    {
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
    }

    private class FileData
    {
        public Dictionary<int, Dictionary<string, Movement>> frames = null;
    }
}
