CREATE TABLE IF NOT EXISTS manuals (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    category VARCHAR(100) NOT NULL,
    tags TEXT[] NULL,
    content TEXT NOT NULL,
    file_data BYTEA NULL,
    file_type VARCHAR(50) NULL,
    file_name VARCHAR(255) NULL,
    upload_date TIMESTAMP NOT NULL DEFAULT NOW(),
    size INTEGER DEFAULT 0,
    source_url VARCHAR(500) NULL,
    search_query VARCHAR(255) NULL
);

CREATE INDEX IF NOT EXISTS idx_manuals_category ON manuals(category);
CREATE INDEX IF NOT EXISTS idx_manuals_upload_date ON manuals(upload_date DESC);
CREATE INDEX IF NOT EXISTS idx_manuals_title ON manuals USING gin(to_tsvector('english', title));
CREATE INDEX IF NOT EXISTS idx_manuals_content ON manuals USING gin(to_tsvector('english', content));
