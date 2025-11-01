import { useEffect, useMemo, useState } from 'react';
import ManualList from './components/ManualList.jsx';
import ManualDetail from './components/ManualDetail.jsx';
import ManualForm from './components/ManualForm.jsx';
import SearchFilters from './components/SearchFilters.jsx';
import AutoFindManual from './components/AutoFindManual.jsx';
import QRScanner from './components/QRScanner.jsx';
import QrGenerator from './components/QrGenerator.jsx';
import { useManuals } from './hooks/useManuals.js';
import { createManual, deleteManual, getManual, listCategories } from './api/client.js';

export default function App() {
  const manualsState = useManuals({ pageSize: 10 });
  const { manuals, loading, error, setSearch, setCategory, pagination, refresh, setPage, filters } = manualsState;

  const [categories, setCategories] = useState([]);
  const [selectedManual, setSelectedManual] = useState(null);
  const [formOpen, setFormOpen] = useState(false);
  const [formDefaults, setFormDefaults] = useState(null);
  const [qrManual, setQrManual] = useState(null);
  const [banner, setBanner] = useState(null);

  useEffect(() => {
    async function fetchCategories() {
      try {
        const data = await listCategories();
        setCategories(data);
      } catch (err) {
        setBanner('Unable to load categories.');
      }
    }
    fetchCategories();
  }, []);

  const handleSelectManual = async id => {
    try {
      const manual = await getManual(id);
      setSelectedManual(manual);
    } catch (err) {
      setBanner('Manual could not be loaded.');
    }
  };

  const handleManualCreated = async payload => {
    await createManual(payload);
    await refresh();
    setFormOpen(false);
    setFormDefaults(null);
    setBanner('Manual saved successfully.');
  };

  const handleDeleteManual = async id => {
    if (!window.confirm('Delete this manual?')) return;
    await deleteManual(id);
    await refresh();
    setSelectedManual(null);
    setBanner('Manual deleted.');
  };

  const handleAutoFindImport = values => {
    setFormDefaults(values);
    setFormOpen(true);
  };

  const handleQrDetected = text => {
    setSearch(text);
    setBanner(`Search updated with QR value: ${text}`);
  };

  useEffect(() => {
    if (manuals.length > 0 && !selectedManual) {
      handleSelectManual(manuals[0].id);
    }
  }, [manuals]);

  const errorMessage = useMemo(() => {
    if (!error) return null;
    return error.message || 'An unexpected error occurred.';
  }, [error]);

  return (
    <div className="app">
      <header className="app__header">
        <h1>ManualMaster</h1>
        <div className="app__header-actions">
          <button className="btn" onClick={() => setFormOpen(true)}>Add Manual</button>
        </div>
      </header>

      {banner && (
        <div className="banner" role="status">
          {banner}
          <button type="button" onClick={() => setBanner(null)} aria-label="Dismiss">
            ×
          </button>
        </div>
      )}
      {errorMessage && <div className="banner banner--error">{errorMessage}</div>}

      <main className="layout">
        <div className="layout__column">
          <SearchFilters
            search={filters.search}
            category={filters.category}
            categories={categories}
            onSearchChange={setSearch}
            onCategoryChange={value => setCategory(value || '')}
            onRefresh={refresh}
          />
          <ManualList
            manuals={manuals}
            selectedId={selectedManual?.id}
            onSelect={handleSelectManual}
            loading={loading}
            pagination={pagination}
            onPageChange={setPage}
          />
        </div>
        <div className="layout__column">
          <ManualDetail manual={selectedManual} onDelete={handleDeleteManual} onGenerateQr={setQrManual} />
          <AutoFindManual onImport={handleAutoFindImport} />
        </div>
        <div className="layout__column">
          {formOpen && (
            <ManualForm
              onSubmit={handleManualCreated}
              categories={categories}
              defaultValues={formDefaults}
              onClose={() => setFormOpen(false)}
            />
          )}
          <QRScanner onDetected={handleQrDetected} />
          {qrManual && <QrGenerator manual={qrManual} onClose={() => setQrManual(null)} />}
        </div>
      </main>
    </div>
  );
}
