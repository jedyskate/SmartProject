FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY ./src /app/src

ENV OLLAMA_BASE_URL="http://host.docker.internal:11434"

ENTRYPOINT ["python", "-m", "src.main"]
CMD ["Write a blog post about the future of AI in content creation."]
