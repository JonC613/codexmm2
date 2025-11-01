import { useState } from 'react';

export default function SearchFilters({ search, category, onSearchChange, onCategoryChange, categories, onRefresh }) {
  const [localSearch, setLocalSearch] = useState(search || '');

  const handleSearchSubmit = event => {
    event.preventDefault();
    onSearchChange(localSearch);
  };

  return (
    <section className="panel">
      <form className="filters" onSubmit={handleSearchSubmit}>
        <div className="filters__group">
          <label htmlFor="search">Search</label>
          <input
            id="search"
            type="search"
            value={localSearch}
            onChange={event => setLocalSearch(event.target.value)}
            placeholder="Search manuals"
          />
        </div>
        <div className="filters__group">
          <label htmlFor="category">Category</label>
          <select
            id="category"
            value={category || ''}
            onChange={event => onCategoryChange(event.target.value)}
          >
            <option value="">All</option>
            {categories.map(cat => (
              <option key={cat} value={cat}>
                {cat}
              </option>
            ))}
          </select>
        </div>
        <div className="filters__actions">
          <button type="submit" className="btn">
            Apply
          </button>
          <button type="button" className="btn btn--secondary" onClick={onRefresh}>
            Refresh
          </button>
        </div>
      </form>
    </section>
  );
}
