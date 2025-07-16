import os
import shutil
from langchain_community.document_loaders import PyPDFLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.schema import Document
from langchain_community.vectorstores import Chroma
from langchain.prompts import ChatPromptTemplate
from langchain_community.llms.ollama import Ollama
from langchain_community.embeddings import HuggingFaceEmbeddings

# Constants
CHROMA_PATH = "chroma"
DATA_PATH = r"C:\Users\AsmaA\Downloads\Link\anotherONE_nlp.pdf"
PROMPT_TEMPLATE = """
You are a virtual tour guide for Egyptian artifacts. Answer the question below based only on the following context:

{context}

---

Question: {question}

Provide a helpful and informative response in the tone of a friendly guide.
"""

# Embedding function
def get_embedding_function():
    embeddings = HuggingFaceEmbeddings(model_name="all-MiniLM-L6-v2")
    return embeddings

# Populate the database
def populate_database(reset=False):
    os.makedirs(CHROMA_PATH, exist_ok=True)  # Ensure the path exists
    if reset:
        print("✨ Clearing Database...")
        clear_database()

    documents = load_documents()
    if not documents:
        print("❌ No documents found in the specified path.")
        return

    chunks = split_documents(documents)
    add_to_chroma(chunks)

def load_documents():
    try:
        loader = PyPDFLoader(DATA_PATH)  # Single PDF file loader
        documents = loader.load()
        print(f"✅ Loaded {len(documents)} documents.")
        return documents
    except Exception as e:
        print(f"Error loading documents: {e}")
        return []

def split_documents(documents: list[Document]):
    text_splitter = RecursiveCharacterTextSplitter(
        chunk_size=800,
        chunk_overlap=80,
        length_function=len,
    )
    chunks = text_splitter.split_documents(documents)
    print(f"✅ Split into {len(chunks)} chunks.")
    return chunks

def add_to_chroma(chunks: list[Document]):
    embedding_function = get_embedding_function()
    db = Chroma(persist_directory=CHROMA_PATH, embedding_function=embedding_function)
    chunks_with_ids = calculate_chunk_ids(chunks)

    existing_ids = set(db.get()["ids"])
    new_chunks = [chunk for chunk in chunks_with_ids if chunk.metadata["id"] not in existing_ids]
    
    print(f"👉 Adding {len(new_chunks)} new documents to the database...")
    if new_chunks:
        db.add_documents(new_chunks, ids=[chunk.metadata["id"] for chunk in new_chunks])
        db.persist()
    else:
        print("✅ No new documents to add.")

def calculate_chunk_ids(chunks: list[Document]):
    for i, chunk in enumerate(chunks):
        source = chunk.metadata.get("source", "unknown")
        page = chunk.metadata.get("page", "0")
        chunk.metadata["id"] = f"{source}:page_{page}:chunk_{i}"
    return chunks

def clear_database():
    if os.path.exists(CHROMA_PATH):
        def onerror(func, path, exc_info):
            import stat
            os.chmod(path, stat.S_IWRITE)  # Change file permissions
            func(path)  # Retry the removal

        shutil.rmtree(CHROMA_PATH, onerror=onerror)
        print("✅ Database cleared.")
    else:
        print("⚠️ Database path does not exist.")

# Query the database
def query_database(query_text):
    embedding_function = get_embedding_function()
    db = Chroma(persist_directory=CHROMA_PATH, embedding_function=embedding_function)

    # Search the database
    results = db.similarity_search_with_score(query_text, k=5)
    if not results:
        print("❌ No relevant context found.")
        return

    context_text = "\n\n---\n\n".join([doc.page_content for doc, _ in results])
    prompt_template = ChatPromptTemplate.from_template(PROMPT_TEMPLATE)
    prompt = prompt_template.format(context=context_text, question=query_text)

    # Use the model to generate a response
    model = Ollama(model="mistral")
    response_text = model.invoke(prompt)

    # Collect sources
    sources = [doc.metadata.get("id", "unknown") for doc, _ in results]
    formatted_response = f"Response:\n{response_text}\n\nSources: {', '.join(sources)}"
    print(formatted_response)
    return response_text

# Real-time interaction
def main():
    print("Welcome to the Virtual Tour Guide System!")
    while True:
        print("\nOptions:")
        print("1. Populate Database")
        print("2. Query Database")
        print("3. Clear Database")
        print("4. Exit")

        choice = input("Enter your choice: ").strip()
        if choice == "1":
            reset = input("Reset the database before populating? (yes/no): ").strip().lower() == "yes"
            populate_database(reset=reset)
        elif choice == "2":
            query_text = input("Enter your query: ").strip()
            query_database(query_text)
        elif choice == "3":
            clear_database()
        elif choice == "4":
            print("Goodbye!")
            break
        else:
            print("Invalid choice. Please try again.")

if __name__ == "__main__":
    main()
