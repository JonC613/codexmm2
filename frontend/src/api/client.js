import axios from 'axios';

const API_BASE = (import.meta.env.VITE_API_BASE || 'http://localhost:5180').replace(/\/$/, '');

const client = axios.create({
  baseURL: API_BASE,
  headers: {
    'Content-Type': 'application/json'
  }
});

export async function listManuals({ search, category, page, pageSize }) {
  const { data } = await client.get('/api/manuals', {
    params: {
      search: search || undefined,
      category: category || undefined,
      page,
      pageSize
    }
  });
  return data;
}

export async function listCategories() {
  const { data } = await client.get('/api/manuals/categories');
  return data;
}

export async function getManual(id) {
  const { data } = await client.get(`/api/manuals/${id}`);
  return data;
}

export async function createManual(payload) {
  const { data } = await client.post('/api/manuals', payload);
  return data;
}

export async function updateManual(id, payload) {
  const { data } = await client.put(`/api/manuals/${id}`, payload);
  return data;
}

export async function deleteManual(id) {
  await client.delete(`/api/manuals/${id}`);
}

export async function autoFindManuals(body) {
  const { data } = await client.post('/api/manuals/auto-find', body);
  return data;
}

export async function generateQr(payload) {
  const { data } = await client.post('/api/qrcodes/generate', payload);
  return data;
}

export async function decodeQr(base64Image) {
  const { data } = await client.post('/api/qrcodes/decode', { base64Image });
  return data;
}

export function manualDownloadUrl(id) {
  return `${API_BASE}/api/manuals/${id}/download`;
}
