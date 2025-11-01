import { useMemo, useState } from 'react';
import { BrowserQRCodeReader } from '@zxing/browser';
import { decodeQr } from '../api/client.js';

export default function QRScanner({ onDetected }) {
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const reader = useMemo(() => new BrowserQRCodeReader(), []);

  const handleFileChange = async event => {
    const file = event.target.files?.[0];
    if (!file) {
      return;
    }

    setError(null);
    setResult(null);

    try {
      const imageUrl = URL.createObjectURL(file);
      try {
        const decodeResult = await reader.decodeFromImageUrl(imageUrl);
        if (decodeResult?.text) {
          setResult(decodeResult.text);
          onDetected?.(decodeResult.text);
          URL.revokeObjectURL(imageUrl);
          return;
        }
      } catch (clientError) {
        // Fallback to backend decoding when client side fails.
        const base64 = await fileToBase64(file);
        const response = await decodeQr(base64);
        if (response?.text) {
          setResult(response.text);
          onDetected?.(response.text);
          return;
        }
        throw clientError;
      }
    } catch (err) {
      setError('Unable to decode QR image. Try a clearer picture.');
    }
  };

  return (
    <section className="panel">
      <header className="panel__header">
        <h2>QR Scanner</h2>
      </header>
      <div className="qr-scanner">
        <input type="file" accept="image/*" capture="environment" onChange={handleFileChange} />
        {result && (
          <p className="success">
            QR Content: <span>{result}</span>
          </p>
        )}
        {error && <p className="error">{error}</p>}
      </div>
    </section>
  );
}

async function fileToBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      const [, data] = reader.result.split(',');
      resolve(data);
    };
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}
