CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE sessions (
    id UUID PRIMARY KEY,
    user_id TEXT NOT NULL,
    data JSONB,
    created_at TIMESTAMP DEFAULT now(),
    updated_at TIMESTAMP DEFAULT now()
);

CREATE TABLE memory (
    id UUID PRIMARY KEY,
    session_id UUID REFERENCES sessions(id),
    content TEXT NOT NULL,
    embedding VECTOR(1536),
    created_at TIMESTAMP DEFAULT now()
);

CREATE INDEX ON memory
USING hnsw (embedding vector_cosine_ops);
