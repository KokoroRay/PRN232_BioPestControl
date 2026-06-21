import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { agriculturalService } from '../../services/agriculturalService';
import { useAuth } from '../../context/AuthContext';

interface ProductOption {
  id: number;
  name: string;
  imageUrl?: string;
}

interface CropOption {
  id: number;
  name: string;
}

interface PestOption {
  id: number;
  name: string;
}

interface DosageResult {
  productName: string;
  dosageRate: string;
  totalProductNeeded: string;
  numberOfTanks: number;
  amountPerTank: string;
}

interface MixabilityResult {
  isSafe: boolean;
  warnings?: string[];
}

const AgriCalculationsPage: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [products, setProducts] = useState<ProductOption[]>([]);
  const [crops, setCrops] = useState<CropOption[]>([]);
  const [pests, setPests] = useState<PestOption[]>([]);
  const [dataWarning, setDataWarning] = useState<string>('');
  const [loading, setLoading] = useState(true);

  // Dosage form state
  const [selectedProductId, setSelectedProductId] = useState<number | ''>('');
  const [selectedCropId, setSelectedCropId] = useState<number | ''>('');
  const [selectedPestId, setSelectedPestId] = useState<number | ''>('');
  const [areaSize, setAreaSize] = useState('');
  const [tankCapacity, setTankCapacity] = useState('');
  const [dosageResult, setDosageResult] = useState<DosageResult | null>(null);
  const [dosageError, setDosageError] = useState('');

  // Mixability state
  const [selectedMixIds, setSelectedMixIds] = useState<number[]>([]);
  const [mixabilityResult, setMixabilityResult] = useState<MixabilityResult | null>(null);
  const [mixabilityError, setMixabilityError] = useState('');

  const selectedProduct = products.find(p => p.id === selectedProductId);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: '/agri-calculations' } });
    }
  }, [isAuthenticated, navigate]);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        const data = await agriculturalService.getCalculationsData();
        setProducts(data.products || []);
        setCrops(data.crops || []);
        setPests(data.pests || []);
        if (data.dataWarning) setDataWarning(data.dataWarning);
      } catch {
        setDataWarning('Failed to load agricultural data.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const handleDosageSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setDosageResult(null);
    setDosageError('');

    if (!selectedProductId || !selectedCropId || !selectedPestId || !areaSize || !tankCapacity) {
      setDosageError('Please fill in all fields.');
      return;
    }

    try {
      const result = await agriculturalService.calculateDosage({
        productId: Number(selectedProductId),
        cropId: Number(selectedCropId),
        pestId: Number(selectedPestId),
        areaSize: Number(areaSize),
        tankCapacity: Number(tankCapacity),
      });
      setDosageResult(result.dosageResult || result);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Calculation failed.';
      setDosageError(msg);
    }
  };

  const toggleMixProduct = (id: number) => {
    setSelectedMixIds(prev =>
      prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id]
    );
    setMixabilityResult(null);
    setMixabilityError('');
  };

  const handleMixabilitySubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMixabilityResult(null);
    setMixabilityError('');

    if (selectedMixIds.length < 2) {
      setMixabilityError('Please select at least 2 products.');
      return;
    }

    try {
      const result = await agriculturalService.checkMixability(selectedMixIds);
      setMixabilityResult(result.mixabilityResult || result);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Check failed.';
      setMixabilityError(msg);
    }
  };

  if (loading) {
    return (
      <div className="agri-page">
        <div className="agri-loading">
          <span className="material-symbols-outlined spin text-4xl">hourglass_empty</span>
          <p>Loading agricultural data...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="agri-page">
      <div className="agri-container">
        <div className="agri-breadcrumbs">
          <Link to="/">Home</Link>
          <span className="material-symbols-outlined text-xs">chevron_right</span>
          <span>Agricultural Calculations</span>
        </div>

        <div className="agri-header">
          <h1>Agricultural Calculations</h1>
          <p>Calculate pesticide dosage and check whether products are safe to mix.</p>
        </div>

        {dataWarning && (
          <div className="agri-warning">{dataWarning}</div>
        )}

        <div className="agri-grid">
          {/* Dosage Calculator */}
          <section className="agri-card">
            <h2>Calculate Drug Dosage</h2>
            <form onSubmit={handleDosageSubmit} className="agri-form">
              <div className="agri-field">
                <label>Product</label>
                <select
                  value={selectedProductId}
                  onChange={e => setSelectedProductId(e.target.value ? Number(e.target.value) : '')}
                >
                  <option value="">Select product</option>
                  {products.map(p => (
                    <option key={p.id} value={p.id}>{p.name}</option>
                  ))}
                </select>
                {selectedProduct && (
                  <div className="agri-product-preview">
                    {selectedProduct.imageUrl ? (
                      <img src={selectedProduct.imageUrl} alt={selectedProduct.name} />
                    ) : (
                      <div className="agri-product-placeholder">N/A</div>
                    )}
                    <span>{selectedProduct.name}</span>
                  </div>
                )}
              </div>

              <div className="agri-row-2">
                <div className="agri-field">
                  <label>Crop</label>
                  <select
                    value={selectedCropId}
                    onChange={e => setSelectedCropId(e.target.value ? Number(e.target.value) : '')}
                  >
                    <option value="">Select crop</option>
                    {crops.map(c => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))}
                  </select>
                </div>
                <div className="agri-field">
                  <label>Pest/Disease</label>
                  <select
                    value={selectedPestId}
                    onChange={e => setSelectedPestId(e.target.value ? Number(e.target.value) : '')}
                  >
                    <option value="">Select pest/disease</option>
                    {pests.map(p => (
                      <option key={p.id} value={p.id}>{p.name}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="agri-row-2">
                <div className="agri-field">
                  <label>Area Size (m²)</label>
                  <input
                    type="number"
                    step="0.01"
                    min="0.01"
                    value={areaSize}
                    onChange={e => setAreaSize(e.target.value)}
                    placeholder="e.g. 500"
                  />
                </div>
                <div className="agri-field">
                  <label>Tank Capacity (liters)</label>
                  <input
                    type="number"
                    step="0.1"
                    min="0.1"
                    value={tankCapacity}
                    onChange={e => setTankCapacity(e.target.value)}
                    placeholder="e.g. 16"
                  />
                </div>
              </div>

              <button type="submit" className="agri-btn-primary">
                Calculate Dosage
              </button>
            </form>

            {dosageError && <div className="agri-error">{dosageError}</div>}

            {dosageResult && (
              <div className="agri-result">
                <h3>Dosage Result</h3>
                <ul>
                  <li><strong>Product:</strong> {dosageResult.productName}</li>
                  <li><strong>Rate:</strong> {dosageResult.dosageRate}</li>
                  <li><strong>Total Product Needed:</strong> {dosageResult.totalProductNeeded}</li>
                  <li><strong>Number of Tanks:</strong> {dosageResult.numberOfTanks}</li>
                  <li><strong>Amount Per Tank:</strong> {dosageResult.amountPerTank}</li>
                </ul>
              </div>
            )}
          </section>

          {/* Mixability Checker */}
          <section className="agri-card">
            <h2>Check Mixability</h2>
            <form onSubmit={handleMixabilitySubmit} className="agri-form">
              <p className="agri-field-hint">Select at least 2 products</p>
              <div className="agri-product-list">
                {products.map(p => (
                  <label key={p.id} className={`agri-product-item ${selectedMixIds.includes(p.id) ? 'selected' : ''}`}>
                    <input
                      type="checkbox"
                      checked={selectedMixIds.includes(p.id)}
                      onChange={() => toggleMixProduct(p.id)}
                    />
                    {p.imageUrl ? (
                      <img src={p.imageUrl} alt={p.name} />
                    ) : (
                      <div className="agri-product-placeholder">N/A</div>
                    )}
                    <span>{p.name}</span>
                  </label>
                ))}
              </div>
              <button type="submit" className="agri-btn-secondary">
                Check Mixability
              </button>
            </form>

            {mixabilityError && <div className="agri-error">{mixabilityError}</div>}

            {mixabilityResult && (
              mixabilityResult.isSafe ? (
                <div className="agri-result agri-result-safe">
                  <h3>Safe to mix</h3>
                  <p>No chemical conflicts were found for the selected products.</p>
                </div>
              ) : (
                <div className="agri-result agri-result-warning">
                  <h3>Mixing Warning</h3>
                  <ul>
                    {(mixabilityResult.warnings || []).map((w, i) => (
                      <li key={i}>{w}</li>
                    ))}
                  </ul>
                </div>
              )
            )}
          </section>
        </div>
      </div>
    </div>
  );
};

export default AgriCalculationsPage;
