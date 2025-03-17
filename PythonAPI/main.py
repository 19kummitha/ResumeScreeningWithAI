from fastapi import FastAPI
import spacy

app = FastAPI()

# Load SpaCy model once at startup
nlp = spacy.load("en_core_web_sm")
print("✅ SpaCy Model Loaded Successfully!")

@app.get("/")
def read_root():
    return {"message": "Python API is working correctly!"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
