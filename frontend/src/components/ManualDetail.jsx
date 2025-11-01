import { useState } from 'react';
import { manualDownloadUrl } from '../api/client.js';

export default function ManualDetail({ manual, onDelete, onGenerateQr }) {
  const [expanded, setExpanded] = useState(false);

  if (!manual) {
    return (
      <section className="panel">
        <header className="panel__header">
          <h2>Details</h2>
        </header>
        <p className="empty">Select a manual to view details.</p>
      </section>
    );
  }

  const toggleExpanded = () => setExpanded(prev => !prev);
  const downloadUrl = manualDownloadUrl(manual.id);

  return (
    <section className="panel">
      <header className="panel__header">
        <h2>{manual.title}</h2>
        <div className="panel__actions">
          <button type="button" onClick={() => onGenerateQr(manual)}>QR Code</button>
          <a className="btn" href={downloadUrl} target="_blank" rel="noopener noreferrer">
            Download
          </a>
          <button type="button" className="btn btn--danger" onClick={() => onDelete(manual.id)}>
            Delete
          </button>
        </div>
      </header>
      <div className="manual-detail">
        <div className="manual-detail__meta">
          <span className="badge">{manual.category}</span>
          {manual.tags?.map(tag => (
            <span key={tag} className="tag">#{tag}</span>
          ))}
        </div>
        {manual.sourceUrl && (
          <p>
            Source: <a href={manual.sourceUrl} target="_blank" rel="noopener noreferrer">{manual.sourceUrl}</a>
          </p>
        )}
        <article className={`manual-detail__content ${expanded ? 'manual-detail__content--expanded' : ''}`}>
          <pre>{manual.content}</pre>
        </article>
        <button type="button" onClick={toggleExpanded} className="btn btn--secondary">
          {expanded ? 'Show Less' : 'Show More'}
        </button>
      </div>
    </section>
  );
}
