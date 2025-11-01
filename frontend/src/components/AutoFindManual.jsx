import { useState } from 'react';
import { autoFindManuals } from '../api/client.js';

export default function AutoFindManual({ onImport }) {
  const [productName, setProductName] = useState('');
  const [modelNumber, setModelNumber] = useState('');
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async event => {
    event.preventDefault();
    if (!productName.trim()) {
      setError('Provide at least a product name.');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const data = await autoFindManuals({ productName, modelNumber });
      setResults(data);
    } catch (err) {
      setError('Failed to search. Ensure the backend auto-find service is configured.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className="panel">
      <header className="panel__header">
        <h2>Auto-Find Manual</h2>
      </header>
      <form className="auto-find" onSubmit={handleSubmit}>
        <input
          value={productName}
          onChange={event => setProductName(event.target.value)}
          placeholder="Product name"
        />
        <input
          value={modelNumber}
          onChange={event => setModelNumber(event.target.value)}
          placeholder="Model number (optional)"
        />
        <button type="submit" className="btn" disabled={loading}>
          {loading ? 'Searching…' : 'Search' }
        </button>
      </form>
      {error && <p className="error">{error}</p>}
      <div className="auto-find__results">
        {results.map(result => (
          <article key={result.sourceUrl} className="auto-find__card">
            <h3>{result.title}</h3>
            <p>{result.snippet}</p>
            <p className="hint">Preview: {result.contentPreview}</p>
            <a href={result.sourceUrl} target="_blank" rel="noopener noreferrer">
              Visit Source
            </a>
            <button
              type="button"
              className="btn btn--secondary"
              onClick={() =>
                onImport({
                  title: result.title,
                  content: result.contentPreview,
                  sourceUrl: result.sourceUrl,
                  searchQuery: `${productName} ${modelNumber}`.trim()
                })
              }
            >
              Use Result
            </button>
          </article>
        ))}
        {!loading && results.length === 0 && <p className="hint">No results yet.</p>}
      </div>
    </section>
  );
}
