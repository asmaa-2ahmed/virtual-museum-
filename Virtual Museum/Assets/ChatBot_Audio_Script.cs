using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChatBot_Audio_Script : MonoBehaviour
{
    public string serverAddress = "192.168.56.1"; // Replace with your Python server's IP
    public int port = 12345; // Same port as the Python server
    public AudioSource audioSource; // AudioSource to play the received audio

    private TcpClient client;
    private NetworkStream stream;
    private AudioClip audioClip;

    void Start()
    {
        try
        {
            // Establish the connection to the server
            client = new TcpClient(serverAddress, port);
            stream = client.GetStream();
            Debug.Log("Connected to Python server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    // Call this function when the mic button is clicked to start recording
    public void RecordAndSendAudio()
    {
        // Start recording audio from the microphone
        audioClip = Microphone.Start(null, false, 10, 44100);
        Debug.Log("Recording started...");

        // Stop recording after a short delay (for example, 3 seconds)
        Invoke("StopAndSendAudio", 3f);  // Stop recording after 3 seconds
    }

    private void StopAndSendAudio()
    {
        // Stop the recording
        Microphone.End(null);
        Debug.Log("Recording stopped.");

        // Get the raw PCM data from the AudioClip
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        // Convert the samples into a byte array
        byte[] audioData = new byte[samples.Length * 4];  // Each sample is 4 bytes (float to byte conversion)
        Buffer.BlockCopy(samples, 0, audioData, 0, audioData.Length);

        // Send the audio data to Python
        SendAudioToServer(audioData);
    }

    private void SendAudioToServer(byte[] audioData)
    {
        try
        {
            // Send the audio data as raw byte array to the server
            stream.Write(audioData, 0, audioData.Length);
            Debug.Log("Audio sent to Python server.");

            // Receive response from Python (audio data back)
            byte[] receivedData = new byte[1024 * 100]; // Increase buffer size if needed
            int bytesRead = stream.Read(receivedData, 0, receivedData.Length);

            // Play the received audio (this should be PCM data)
            PlayReceivedAudio(receivedData, bytesRead);
        }
        catch (Exception e)
        {
            Debug.LogError("Error during audio communication: " + e.Message);
        }
    }

    private void PlayReceivedAudio(byte[] audioData, int bytesRead)
    {
        // Convert the received byte array back to float samples
        float[] receivedSamples = new float[bytesRead / 4]; // 4 bytes per sample
        Buffer.BlockCopy(audioData, 0, receivedSamples, 0, bytesRead);

        // Create an AudioClip from the byte array
        AudioClip clip = AudioClip.Create("Received Audio", receivedSamples.Length, 1, 44100, false);
        clip.SetData(receivedSamples, 0);

        // Play the received audio
        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log("Playing received audio...");
    }

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
        Debug.Log("Connection closed.");
    }
}
