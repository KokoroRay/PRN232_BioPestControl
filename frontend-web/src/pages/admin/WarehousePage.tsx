import React, { useCallback, useEffect, useState } from 'react';
import { History } from 'lucide-react';
import { PageHeader } from '../../components/admin/PageHeader';
import { LoadingState } from '../../components/admin/LoadingState';
import { Drawer } from '../../components/admin/Drawer';
import { useToast } from '../../context/ToastContext';
import { usePageMode } from '../../context/PageModeContext';
import { inventoryService } from '../../services/inventoryService';
import { productService } from '../../services/productService';
import type { ProductDetail, ProductStock, WarehouseImport } from '../../types/inventory';
import type { Product } from '../../types/catalog';
import * as XLSX from 'xlsx';

const formatDate = (value: string) => {
  const d = new Date(value);
  return Number.isNaN(d.getTime()) ? value : d.toLocaleString();
};

const formatMoney = (n: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(n);

const WarehousePage: React.FC = () => {
  const { canImportWarehouse } = usePageMode();
  const { showToast } = useToast();
  const [stock, setStock] = useState<ProductStock[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [sku, setSku] = useState('');
  const [qty, setQty] = useState(1);
  const [price, setPrice] = useState(0);
  const [note, setNote] = useState('');
  const [importing, setImporting] = useState(false);
  const [historyOpen, setHistoryOpen] = useState(false);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [detail, setDetail] = useState<ProductDetail | null>(null);

  // Pagination
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [s, pRes] = await Promise.all([
        inventoryService.getStock(search || undefined, page, pageSize),
        productService.getAll({ pageSize: 100 }), // Get max for dropdown
      ]);
      setStock(s.items);
      setTotalCount(s.totalCount);
      setProducts(pRes.items);
    } catch {
      showToast('Failed to load warehouse data', 'error');
    } finally {
      setLoading(false);
    }
  }, [search, page, pageSize, showToast]);

  useEffect(() => {
    const t = setTimeout(load, 300);
    return () => clearTimeout(t);
  }, [load]);

  const totalPages = Math.ceil(totalCount / pageSize);

  const openHistory = async (productId: number) => {
    setHistoryOpen(true);
    setHistoryLoading(true);
    setDetail(null);
    try {
      setDetail(await inventoryService.getById(productId));
    } catch {
      showToast('Failed to load import history', 'error');
      setHistoryOpen(false);
    } finally {
      setHistoryLoading(false);
    }
  };

  const closeHistory = () => {
    setHistoryOpen(false);
    setDetail(null);
  };

  const handleImport = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!sku.trim()) {
      showToast('Select a product SKU', 'error');
      return;
    }
    setImporting(true);
    try {
      await inventoryService.importProducts(
        [{ sku: sku.trim(), quantity: qty, importPrice: price }],
        note || undefined,
      );
      showToast('Import successful');
      setSku('');
      setQty(1);
      setNote('');
      load();
    } catch {
      showToast('Import failed', 'error');
    } finally {
      setImporting(false);
    }
  };

  const handleExcelImport = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setImporting(true);
    try {
      const data = await file.arrayBuffer();
      const workbook = XLSX.read(data);
      const firstSheetName = workbook.SheetNames[0];
      const worksheet = workbook.Sheets[firstSheetName];
      // Expected columns: SKU, Quantity, Price
      const json = XLSX.utils.sheet_to_json(worksheet) as any[];

      const items = json.map(row => ({
        sku: String(row.SKU || row.sku || ''),
        quantity: Number(row.Quantity || row.quantity),
        importPrice: Number(row.Price || row.price || row.importPrice)
      })).filter(item => item.sku && !isNaN(item.quantity) && !isNaN(item.importPrice));

      if (items.length === 0) {
        showToast('No valid data found in Excel', 'error');
        return;
      }

      await inventoryService.importProducts(items, 'Excel Import');
      showToast(`Imported ${items.length} products successfully`);
      load();
    } catch (err) {
      showToast('Failed to parse or import Excel', 'error');
    } finally {
      setImporting(false);
      // Reset input
      e.target.value = '';
    }
  };

  const handleDownloadTemplate = () => {
    const ws = XLSX.utils.json_to_sheet([
      { SKU: 'SP-01', Quantity: 100, Price: 50000 }
    ]);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Template");
    XLSX.writeFile(wb, "Import_Template.xlsx");
  };

  const history: WarehouseImport[] = detail?.importHistory ?? [];

  return (
    <div className="admin-page">
      <PageHeader
        title="Warehouse"
        subtitle={
          canImportWarehouse
            ? 'Stock levels, imports, and per-product import history.'
            : 'View stock levels and import history (read-only).'
        }
      />
      {canImportWarehouse && (
        <div className="panel-card import-panel">
          <h3 className="import-panel-title">Manual Import</h3>
          <form onSubmit={handleImport} className="import-form">
            <label className="import-field import-field--product">
              <span className="import-field-label">Product</span>
              <select value={sku} onChange={(e) => setSku(e.target.value)} required>
                <option value="">Select product</option>
                {products.map((p) => (
                  <option key={p.id} value={p.sku}>
                    {p.name} ({p.sku})
                  </option>
                ))}
              </select>
            </label>
            <label className="import-field">
              <span className="import-field-label">Quantity</span>
              <input
                type="number"
                min={1}
                value={qty}
                onChange={(e) => setQty(Number(e.target.value))}
              />
            </label>
            <label className="import-field">
              <span className="import-field-label">Import price</span>
              <input
                type="number"
                min={0}
                step={1000}
                value={price}
                onChange={(e) => setPrice(Number(e.target.value))}
              />
            </label>
            <label className="import-field import-field--note">
              <span className="import-field-label">Note</span>
              <input
                value={note}
                onChange={(e) => setNote(e.target.value)}
                placeholder="Optional"
              />
            </label>
            <button type="submit" className="btn-primary import-submit" disabled={importing}>
              {importing ? 'Importing...' : 'Import'}
            </button>
          </form>
          <div className="flex gap-4 mt-6 pt-6 border-t border-outline-variant/10">
            <label className="btn-secondary cursor-pointer inline-flex items-center gap-2">
              <span className="material-symbols-outlined text-sm">upload_file</span>
              {importing ? 'Importing...' : 'Import from Excel'}
              <input
                type="file"
                accept=".xlsx, .xls"
                onChange={handleExcelImport}
                disabled={importing}
                className="hidden"
              />
            </label>
            <button
              type="button"
              className="text-primary text-sm font-medium hover:underline inline-flex items-center gap-1"
              onClick={handleDownloadTemplate}
            >
              <span className="material-symbols-outlined text-sm">download</span>
              Download Template
            </button>
          </div>
        </div>
      )}
      <div className="filter-bar">
        <input
          placeholder="Search stock..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>
      {loading ? (
        <LoadingState />
      ) : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Product</th>
                <th>SKU</th>
                <th>Stock</th>
                <th>Status</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {stock.map((s) => (
                <tr key={s.id} className={s.isLowStock ? 'row-warning' : ''}>
                  <td>{s.name}</td>
                  <td>{s.sku}</td>
                  <td>
                    <strong>{s.stockQuantity}</strong>
                  </td>
                  <td>
                    {s.isLowStock ? (
                      <span className="pill pill-red">Low</span>
                    ) : (
                      <span className="pill pill-green">OK</span>
                    )}
                  </td>
                  <td className="actions-cell">
                    <button
                      type="button"
                      className="btn-icon"
                      onClick={() => openHistory(s.id)}
                      aria-label={`Import history for ${s.name}`}
                      title="Import history"
                    >
                      <History size={18} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {totalPages > 1 && (
            <div className="pagination flex items-center justify-between p-4 border-t border-outline-variant/10">
              <button 
                type="button" 
                className="btn-secondary" 
                disabled={page <= 1} 
                onClick={() => setPage(p => p - 1)}>
                Previous
              </button>
              <span className="text-sm">Page {page} of {totalPages}</span>
              <button 
                type="button" 
                className="btn-secondary" 
                disabled={page >= totalPages} 
                onClick={() => setPage(p => p + 1)}>
                Next
              </button>
            </div>
          )}
        </div>
      )}

      <Drawer
        open={historyOpen}
        title={detail ? `Import history — ${detail.name}` : 'Import history'}
        wide
        onClose={closeHistory}
      >
        {historyLoading ? (
          <LoadingState />
        ) : detail ? (
          <>
            <div className="stats-row">
              <div className="mini-stat">
                <span>Current stock</span>
                <strong>{detail.stockQuantity}</strong>
              </div>
              <div className="mini-stat">
                <span>Import entries</span>
                <strong>{history.length}</strong>
              </div>
            </div>
            {history.length === 0 ? (
              <p className="text-muted">No import records for this product yet.</p>
            ) : (
              <div className="data-table-wrap">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Batch</th>
                      <th>Qty</th>
                      <th>Price</th>
                      <th>Supplier</th>
                      <th>By</th>
                      <th>Date</th>
                      <th>Note</th>
                    </tr>
                  </thead>
                  <tbody>
                    {history.map((h) => (
                      <tr key={h.id}>
                        <td>{h.batchCode}</td>
                        <td>{h.quantityImported}</td>
                        <td>{formatMoney(h.importPrice)}</td>
                        <td>{h.supplierName ?? '—'}</td>
                        <td>{h.importedByUserName ?? '—'}</td>
                        <td>{formatDate(h.importedAt)}</td>
                        <td>{h.note ?? '—'}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </>
        ) : null}
      </Drawer>
    </div>
  );
};

export default WarehousePage;
