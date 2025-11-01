import { useState } from 'react';
import { generateQr } from '../api/client.js';

export default function QrGenerator({ manual, onClose }) {
  const [qr, setQr] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  if (!manual) {
    return null;
  }

  const handleGenerate = async () => {
    try {
      setLoading(true);
      setError(null);
      const payload = {
        payload: manual.sourceUrl || `${location.origin}/manuals/${manual.id}`,
        pixelsPerModule: 10
      };
      const response = await generateQr(payload);
      setQr(`data:image/png;base64,${response.base64Png}`);
    } catch (err) {
      setError('Failed to generate QR.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className="panel">
      <header className="panel__header">
        <h2>QR Code for {manual.title}</h2>
        <button type="button" className="btn btn--secondary" onClick={onClose}>
          Close
        </button>
      </header>
      <div className="qr-generator">
        <p className="hint">
          Generates a QR using the manual source URL or a fallback local URL.
        </p>
        <button type="button" className="btn" onClick={handleGenerate} disabled={loading}>
          {loading ? 'Generating…' : 'Generate QR'}
        </button>
        {error && <p className="error">{error}</p>}
        {qr && (
          <div className="qr-generator__preview">
            <img src={qr} alt="Manual QR" />
            <a className="btn" href={qr} download={`manual-${manual.id}.png`}>
              Download PNG
            </a>
          </div>
        )}
      </div>
    </section>
  );
}
