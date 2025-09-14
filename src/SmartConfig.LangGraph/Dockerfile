FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY ./app /app/app

ENV OLLAMA_BASE_URL="http://host.docker.internal:11434"

CMD ["uvicorn", "app.main:api", "--host", "0.0.0.0", "--port", "8000"]
