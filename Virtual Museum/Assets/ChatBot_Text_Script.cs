using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Networking; // For UnityWebRequest

public class ChatBot_Text_Script : MonoBehaviour
{
    public InputField inputField;    // Link your Input Field
    public Button sendButton;       // Link your Button
    public Text responseText;       // Link your Response Text Box
    public AudioSource audioSource; // Link your AudioSource for playing audio

    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        // Connect to the Python server
        ConnectToServer();

        // Add listener to button
        sendButton.onClick.AddListener(SendMessageToPython);
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 1234); // Adjust the IP and port as needed
            stream = client.GetStream();
            Debug.Log("Connected to the server.");
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e.Message);
        }
    }

    public void SendMessageToPython()
    {
        if (client == null || !client.Connected)
        {
            Debug.LogError("Not connected to the server.");
            return;
        }

        string message = inputField.text;

        if (!string.IsNullOrEmpty(message))
        {
            try
            {
                // Send message
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                stream.Write(data, 0, data.Length);

                // Receive text response
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                responseText.text = "Response: " + response;

                // Wait for the audio notification
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string notification = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                if (notification == "AUDIO_INCOMING")
                {
                    // Receive audio data
                    ReceiveAudioData();
                }
                else
                {
                    Debug.LogError("Unexpected server response: " + notification);
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Error communicating with the server: " + e.Message);
            }
        }
    }

    void ReceiveAudioData()
    {
        try
        {
            // Create a temporary file to store the received audio
            string tempFilePath = Path.Combine(Application.persistentDataPath, "response.wav");
            using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);

                    // Stop reading when the server closes the audio stream
                    if (bytesRead < buffer.Length)
                        break;
                }
            }

            Debug.Log("Audio received and saved as response.wav");

            // Play the audio file
            StartCoroutine(PlayAudio(tempFilePath));
        }
        catch (IOException e)
        {
            Debug.LogError("Error receiving audio: " + e.Message);
        }
    }

    System.Collections.IEnumerator PlayAudio(string filePath)
    {
        // Load the audio file using UnityWebRequest
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading audio file: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    Debug.Log("Playing audio response.");
                }
                else
                {
                    Debug.LogError("Failed to load audio clip.");
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
