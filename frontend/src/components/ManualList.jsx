import { format } from 'date-fns';

export default function ManualList({ manuals, selectedId, onSelect, loading, pagination, onPageChange }) {
  return (
    <section className="panel">
      <header className="panel__header">
        <h2>Manuals</h2>
        {loading && <span className="badge">Loading…</span>}
      </header>
      <div className="manual-list">
        {manuals.length === 0 && !loading && <p className="empty">No manuals found.</p>}
        {manuals.map(manual => (
          <article
            key={manual.id}
            className={`manual-card ${selectedId === manual.id ? 'manual-card--active' : ''}`}
            onClick={() => onSelect(manual.id)}
          >
            <div className="manual-card__header">
              <h3>{manual.title}</h3>
              <span className="manual-card__category">{manual.category}</span>
            </div>
            <div className="manual-card__tags">
              {manual.tags?.map(tag => (
                <span key={tag} className="tag">#{tag}</span>
              ))}
            </div>
            <div className="manual-card__meta">
              <span>{format(new Date(manual.uploadDate), 'MMM d, yyyy')}</span>
              <span>{manual.size ? formatBytes(manual.size) : '—'}</span>
            </div>
          </article>
        ))}
      </div>
      {pagination && pagination.pageCount > 1 && (
        <footer className="panel__footer">
          <button
            type="button"
            disabled={pagination.page <= 1}
            onClick={() => onPageChange(pagination.page - 1)}
          >
            Prev
          </button>
          <span>
            Page {pagination.page} / {pagination.pageCount}
          </span>
          <button
            type="button"
            disabled={pagination.page >= pagination.pageCount}
            onClick={() => onPageChange(pagination.page + 1)}
          >
            Next
          </button>
        </footer>
      )}
    </section>
  );
}

function formatBytes(bytes) {
  if (!bytes) return '0 B';
  const sizes = ['B', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(1024));
  return `${(bytes / Math.pow(1024, i)).toFixed(1)} ${sizes[i]}`;
}
