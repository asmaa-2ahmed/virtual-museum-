# 🏛️ Virtual Museum

🎥 **[Watch Output Demo](final video.mp4)**  

> A virtual museum experience where you explore **Ancient Egyptian artifacts** in **3D VR** and ask questions using an intelligent **AI Question-Answering (QA)** system powered by **RAG (Retrieval-Augmented Generation)**.

---

## 🎯 Objective

This project aims to:

- Transform cultural learning using immersive Virtual Reality (VR).
- Leverage **Natural Language Processing (NLP)** and **3D modeling** for interactive experiences.
- Allow users to **ask questions** about Egyptian artifacts (like *Nefertiti*, *Ramesses III*) via **voice or text**.
- Make educational content more **accessible**, **engaging**, and **globally available**.

---

## 🧠 Technologies Used

| Component       | Technology                                           |
|----------------|------------------------------------------------------|
| Backend (NLP)   | Python, Chroma DB, HuggingFace, Ollama (Mistral)    |
| Frontend (VR)   | Unity 3D, C#, VR Controller Input                    |
| Communication   | TCP Socket (Python ↔ Unity)                         |
| TTS (Optional)  | `pyttsx3` for text-to-speech audio responses        |

---

## 🗺️ NLP System Flow (Python)

1. **Initialize Program**  
   - Use command-line arguments to `--populate`, `--query`, or `--evaluate`.

2. **Populate Database**
   - Clear old data if `--reset` is passed.
   - Load PDFs using `PyPDFDirectoryLoader`.
   - Split text into chunks with `RecursiveCharacterTextSplitter`.
   - Embed with HuggingFace embeddings.
   - Store in Chroma vector DB.

3. **Querying**
   - User sends query from Unity.
   - Similar chunks retrieved using similarity search.
   - Prompt formed and passed to **Ollama (Mistral)**.
   - Response returned to Unity with sources.

4. **Evaluation**
   - Retrieval: Recall@K, Precision@K
   - Generation: BLEU, ROUGE
   - End-to-End: Relevance + factual consistency

---

## 🧑‍🎨 Unity VR Museum Flow

1. **User Entry**  
   - User enters immersive 3D VR museum.

2. **Exploration**  
   - Navigate freely and view detailed 3D sculptures:
     - Nefertiti bust  
     - Egyptian cat  
     - Ramesses III statue  
     - Ancient oil lamp  

3. **Interaction**  
   - Grab, move, and rotate artifacts using VR controller.

4. **Asking Questions**  
   - Use floating input bar (voice/text):  
     _“Tell me about Nefertiti”_

5. **AI Response**  
   - Backend generates accurate answer.
   - Response shown as text or played as audio.

6. **Continuous Exploration**  
   - User continues exploring, asking, and learning.

---

## 🔄 Communication: Unity ↔ Python

1. Unity sends UTF-8 encoded query via TCP socket.
2. Python backend:
   - Retrieves context from Chroma DB.
   - Generates response with **Ollama + Mistral**.
3. Text (and optional audio) returned to Unity.
4. Unity displays response and plays audio.

---

## ⚙️ Backend Example Code

### Python TCP Socket

```python
import socket

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(("127.0.0.1", 5000))
s.listen(5)
print("Server listening on port 5000...")
```

---

### Unity TCP Client (C#)

```csharp
TcpClient client = new TcpClient("127.0.0.1", 5000);
NetworkStream stream = client.GetStream();
```

---

## ✅ Why QA (Not Chatbot)?

| Feature         | QA System 🧠               | Chatbot 💬                        |
|----------------|----------------------------|----------------------------------|
| Goal           | Direct factual answers     | General conversation             |
| Interaction    | One-shot Q&A               | Multi-turn dialogue              |
| Best For       | Museums, educational tools | Customer support, helpdesks      |
| Accuracy       | High (fact-based)          | Flexible but less focused        |
| Complexity     | Lower                      | Higher (maintain context)        |

---

## ✅ Why Use RAG?

| Feature               | RAG ✅         | TF-IDF + LLM ⚠️ | BM25 + LLM ⚠️  |
|----------------------|---------------|------------------|----------------|
| Context Understanding | High (semantic) | Low              | Medium         |
| Synonym Handling      | Excellent       | Poor             | Moderate       |
| Scalability           | High            | Medium           | High           |
| Answer Quality        | Very High       | Moderate         | Good           |
| BLEU Score            | 0.7254          | 0.5232           | 0.4760         |

---

## 🗃️ Why Chroma + Ollama + Mistral?

- **Chroma**: Efficient vector storage and semantic search.
- **Ollama**: Fast, prompt-based response generation.
- **Mistral**: Lightweight yet powerful LLM for factual QA.
- **HuggingFaceEmbeddings**: Robust sentence representations.

---

## 📊 Evaluation Metrics

| Metric         | Description                                |
|----------------|--------------------------------------------|
| **Recall@K**   | Did the system retrieve the right chunk?   |
| **Precision@K**| Are top chunks relevant?                   |
| **BLEU Score** | Precision match to human-written answer    |
| **ROUGE Score**| Recall-based overlap with reference answer |

---

## 📚 Why Custom PDF Dataset?

- Consistent formatting of artifact info.
- Embeds images and metadata.
- Easier to extract using text splitter.
- Supports multilingual and multimedia expansion.
- Tamper-proof & compatible with NLP libraries.

---

## 🔊 Optional: Text-to-Speech (TTS)

- Python backend uses `pyttsx3` to synthesize answers.
- Unity downloads and plays .wav files via `AudioSource`.

---

## 🚀 How to Run

### 1. Start Python Server

```bash
python main.py --populate --reset
python main.py --query
```

### 2. Start Unity

- Open the Unity project.
- Click Play and explore the VR Museum.
- Ask questions via input bar or microphone.

---

## 📥 Libraries Used

- `langchain`, `langchain_community`
- `chromadb`, `huggingface_hub`
- `PyPDFDirectoryLoader`, `pyttsx3`, `argparse`, `fuzzywuzzy`
- Unity: `TcpClient`, `AudioSource`, `InputField`, `Button`, etc.

---

## 📽️ Output Video Demo

📺 [Attach Your Video Here](https://github.com/asmaa-2ahmed/virtual-museum-/blob/main/output%20video.webm)

---

## 🧱 Future Enhancements

- 🎤 Speech-to-Text
- 🌍 Multilingual Q&A
- 🎧 Real-time voice narration
- 📊 Admin analytics dashboard
- 👥 Multi-user virtual tours

---

## 💬 Contributing

We welcome pull requests and feedback!  
Build smarter, more inclusive museums with us. 🌍
