FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY ./src /app/src

ENV OLLAMA_BASE_URL="http://host.docker.internal:11434"

# Expose the port the app runs on
EXPOSE 8000

# Run the web server
CMD ["uvicorn", "src.main:api", "--host", "0.0.0.0", "--port", "8000"]