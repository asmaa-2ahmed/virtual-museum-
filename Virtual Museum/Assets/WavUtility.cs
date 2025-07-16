using System;
using UnityEngine;

public static class WavUtility
{
    public static AudioClip ToAudioClip(byte[] wavFile, int offset, int length)
    {
        const int headerSize = 44; // WAV header size
        offset += headerSize;

        // Extract audio data
        float[] audioData = ConvertByteToFloatArray(wavFile, offset, length - headerSize);

        // Create AudioClip
        int sampleCount = audioData.Length;
        int frequency = 44100; // Default frequency (can adjust based on your WAV file)
        AudioClip audioClip = AudioClip.Create("GeneratedAudio", sampleCount, 1, frequency, false);
        audioClip.SetData(audioData, 0);

        return audioClip;
    }

    private static float[] ConvertByteToFloatArray(byte[] byteArray, int offset, int length)
    {
        int floatCount = length / 2; // Assuming 16-bit audio (2 bytes per sample)
        float[] floatArray = new float[floatCount];

        for (int i = 0; i < floatCount; i++)
        {
            short sample = BitConverter.ToInt16(byteArray, offset + (i * 2));
            floatArray[i] = sample / 32768.0f; // Normalize to range [-1, 1]
        }

        return floatArray;
    }
}
