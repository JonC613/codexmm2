import { useEffect, useState } from 'react';

const initialState = {
  title: '',
  category: '',
  tags: '',
  content: '',
  sourceUrl: '',
  searchQuery: ''
};

export default function ManualForm({ onSubmit, categories, defaultValues, onClose }) {
  const [form, setForm] = useState(initialState);
  const [base64FileData, setBase64FileData] = useState(null);
  const [fileName, setFileName] = useState(null);
  const [fileType, setFileType] = useState(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (defaultValues) {
      setForm({
        title: defaultValues.title || '',
        category: defaultValues.category || '',
        tags: defaultValues.tags ? defaultValues.tags.join(', ') : '',
        content: defaultValues.content || '',
        sourceUrl: defaultValues.sourceUrl || '',
        searchQuery: defaultValues.searchQuery || ''
      });
      setBase64FileData(null);
      setFileName(defaultValues.fileName || null);
      setFileType(defaultValues.fileType || null);
    }
  }, [defaultValues]);

  const handleChange = event => {
    const { name, value } = event.target;
    setForm(prev => ({ ...prev, [name]: value }));
  };

  async function handleFileChange(event) {
    const file = event.target.files?.[0];
    if (!file) {
      return;
    }

    try {
      setError(null);
      const base64 = await readFileAsBase64(file);
      setBase64FileData(base64);
      setFileName(file.name);
      setFileType(file.type || undefined);
      if (file.type === 'text/plain') {
        const text = await file.text();
        setForm(prev => ({ ...prev, content: text }));
      }
    } catch (err) {
      setError('Failed to read file.');
    }
  }

  const handleSubmit = async event => {
    event.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const payload = {
        title: form.title,
        category: form.category,
        tags: form.tags
          .split(',')
          .map(tag => tag.trim())
          .filter(Boolean),
        content: form.content,
        base64FileData,
        fileName,
        fileType,
        sourceUrl: form.sourceUrl,
        searchQuery: form.searchQuery
      };
      await onSubmit(payload);
      setForm(initialState);
      setBase64FileData(null);
      setFileName(null);
      setFileType(null);
      if (onClose) {
        onClose();
      }
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to save manual.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <section className="panel">
      <header className="panel__header">
        <h2>Add Manual</h2>
        {onClose && (
          <button type="button" className="btn btn--secondary" onClick={onClose}>
            Close
          </button>
        )}
      </header>
      <form className="manual-form" onSubmit={handleSubmit}>
        <label>
          Title
          <input name="title" value={form.title} onChange={handleChange} required />
        </label>
        <label>
          Category
          <input
            name="category"
            list="category-options"
            value={form.category}
            onChange={handleChange}
            required
          />
          <datalist id="category-options">
            {categories.map(category => (
              <option key={category} value={category} />
            ))}
          </datalist>
        </label>
        <label>
          Tags (comma separated)
          <input name="tags" value={form.tags} onChange={handleChange} placeholder="coffee, kitchen, smart" />
        </label>
        <label>
          Content
          <textarea
            name="content"
            rows="6"
            value={form.content}
            onChange={handleChange}
            placeholder="Paste manual text or rely on PDF extraction."
          />
        </label>
        <label>
          Source URL
          <input name="sourceUrl" value={form.sourceUrl} onChange={handleChange} placeholder="https://" />
        </label>
        <label>
          Search Query
          <input name="searchQuery" value={form.searchQuery} onChange={handleChange} />
        </label>
        <label className="file-input">
          Upload PDF or TXT
          <input type="file" accept=".pdf,.txt,application/pdf,text/plain" onChange={handleFileChange} />
        </label>
        {fileName && <p className="hint">Selected file: {fileName}</p>}
        {error && <p className="error">{error}</p>}
        <button type="submit" className="btn" disabled={busy}>
          {busy ? 'Saving…' : 'Save Manual'}
        </button>
      </form>
    </section>
  );
}

function readFileAsBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result.split(',')[1]);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}
