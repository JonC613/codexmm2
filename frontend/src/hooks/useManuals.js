import { useCallback, useEffect, useMemo, useState } from 'react';
import { listManuals } from '../api/client.js';

const DEFAULT_PAGE_SIZE = 10;

export function useManuals(initialFilters = {}) {
  const [manuals, setManuals] = useState([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(initialFilters.page || 1);
  const [pageSize, setPageSize] = useState(initialFilters.pageSize || DEFAULT_PAGE_SIZE);
  const [filters, setFilters] = useState({
    search: initialFilters.search || '',
    category: initialFilters.category || ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const loadManuals = useCallback(
    async (override = {}) => {
      try {
        setLoading(true);
        setError(null);
        const request = {
          search: override.search ?? filters.search,
          category: override.category ?? filters.category,
          page: override.page || page,
          pageSize: override.pageSize || pageSize
        };
        const response = await listManuals(request);
        setManuals(response.items || []);
        setTotal(response.total || 0);
        setPage(request.page);
        setPageSize(request.pageSize);
      } catch (err) {
        setError(err);
      } finally {
        setLoading(false);
      }
    },
    [filters.category, filters.search, page, pageSize]
  );

  useEffect(() => {
    loadManuals();
  }, [filters.category, filters.search, page, pageSize, loadManuals]);

  const pagination = useMemo(
    () => ({
      page,
      pageSize,
      total,
      pageCount: Math.max(1, Math.ceil(total / pageSize))
    }),
    [page, pageSize, total]
  );

  const setSearch = useCallback(
    value => {
      setFilters(prev => ({ ...prev, search: value }));
      setPage(1);
    },
    []
  );

  const setCategory = useCallback(
    value => {
      setFilters(prev => ({ ...prev, category: value }));
      setPage(1);
    },
    []
  );

  const refresh = useCallback(() => loadManuals(), [loadManuals]);

  return {
    manuals,
    loading,
    error,
    filters,
    setFilters,
    setSearch,
    setCategory,
    page,
    setPage,
    pageSize,
    setPageSize,
    pagination,
    refresh
  };
}
