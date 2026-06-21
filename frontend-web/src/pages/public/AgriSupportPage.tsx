import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { agriculturalService } from '../../services/agriculturalService';
import { useAuth } from '../../context/AuthContext';

interface ProvinceDto {
  id: number;
  name: string;
}

interface DistrictDto {
  id: number;
  name: string;
}

interface WeatherInfo {
  province: string;
  district: string;
  temperature: number;
  humidity: number;
  windSpeed: number;
  description: string;
  advice: string;
}

const AgriSupportPage: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [provinces, setProvinces] = useState<ProvinceDto[]>([]);
  const [districts, setDistricts] = useState<DistrictDto[]>([]);
  const [loading, setLoading] = useState(true);

  const [selectedProvinceId, setSelectedProvinceId] = useState<number | ''>('');
  const [selectedDistrictId, setSelectedDistrictId] = useState<number | ''>('');
  const [weather, setWeather] = useState<WeatherInfo | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true, state: { from: '/agri-support' } });
    }
  }, [isAuthenticated, navigate]);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);
        // GET /AgriSupport returns provinces list
        const data = await agriculturalService.getProvinces();
        setProvinces(data.provinces || []);
      } catch {
        setError('Failed to load province data.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  // When province changes, load districts
  useEffect(() => {
    if (!selectedProvinceId) {
      setDistricts([]);
      setSelectedDistrictId('');
      return;
    }
    const loadDistricts = async () => {
      try {
        const data = await agriculturalService.getDistricts(Number(selectedProvinceId));
        setDistricts(data || []);
        setSelectedDistrictId('');
      } catch {
        setDistricts([]);
        setError('Failed to load districts.');
      }
    };
    loadDistricts();
  }, [selectedProvinceId]);

  const handleLocationSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setWeather(null);
    setError('');

    if (!selectedProvinceId || !selectedDistrictId) {
      setError('Please select both province and district.');
      return;
    }

    try {
      const data = await agriculturalService.getWeatherByLocation(
        Number(selectedProvinceId),
        Number(selectedDistrictId)
      );
      setWeather(data);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Failed to get weather data.';
      setError(msg);
    }
  };

  const handleMyAddressSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setWeather(null);
    setError('');

    try {
      const data = await agriculturalService.getWeatherByMyAddress();
      setWeather(data);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Failed to get weather by your address.';
      setError(msg);
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
          <span>Agricultural Support</span>
        </div>

        <div className="agri-header">
          <h1>Agricultural Support</h1>
          <p>View weather conditions and spraying advice for your area.</p>
        </div>

        <div className="agri-grid">
          {/* Weather by Location */}
          <section className="agri-card">
            <h2>Weather by Location</h2>
            <form onSubmit={handleLocationSubmit} className="agri-form">
              <div className="agri-field">
                <label>Province</label>
                <select
                  value={selectedProvinceId}
                  onChange={e => setSelectedProvinceId(e.target.value ? Number(e.target.value) : '')}
                >
                  <option value="">Select province</option>
                  {provinces.map(p => (
                    <option key={p.id} value={p.id}>{p.name}</option>
                  ))}
                </select>
              </div>

              <div className="agri-field">
                <label>District</label>
                <select
                  value={selectedDistrictId}
                  onChange={e => setSelectedDistrictId(e.target.value ? Number(e.target.value) : '')}
                  disabled={!selectedProvinceId}
                >
                  <option value="">Select district</option>
                  {districts.map(d => (
                    <option key={d.id} value={d.id}>{d.name}</option>
                  ))}
                </select>
              </div>

              <button type="submit" className="agri-btn-primary">
                Get Weather
              </button>
            </form>
          </section>

          {/* Weather by My Address */}
          <section className="agri-card">
            <h2>Weather by My Address</h2>
            <p className="agri-field-hint">
              Use your saved profile location to fetch weather quickly.
            </p>
            <p className="agri-field-hint" style={{ fontSize: '0.7rem', color: '#94a9b8' }}>
              Best accuracy: update Profile with Province + District. If only Address text exists,
              system will auto-detect district/province in Vietnam.
            </p>
            <form onSubmit={handleMyAddressSubmit}>
              <button type="submit" className="agri-btn-secondary">
                Use My Address
              </button>
            </form>
          </section>
        </div>

        {error && <div className="agri-error">{error}</div>}

        {weather && (
          <section className="agri-weather-card">
            <h2>Current Weather</h2>
            <div className="agri-weather-grid">
              <div className="agri-weather-item">
                <p className="agri-weather-label">Location</p>
                <p className="agri-weather-value">{weather.province} - {weather.district}</p>
              </div>
              <div className="agri-weather-item">
                <p className="agri-weather-label">Temperature</p>
                <p className="agri-weather-value">{weather.temperature.toFixed(1)} °C</p>
              </div>
              <div className="agri-weather-item">
                <p className="agri-weather-label">Humidity</p>
                <p className="agri-weather-value">{weather.humidity}%</p>
              </div>
              <div className="agri-weather-item">
                <p className="agri-weather-label">Wind Speed</p>
                <p className="agri-weather-value">{weather.windSpeed.toFixed(1)} m/s</p>
              </div>
            </div>

            <div className="agri-weather-advice">
              <p><strong>Condition:</strong> {weather.description}</p>
              <p><strong>Advice:</strong> {weather.advice}</p>
            </div>
          </section>
        )}
      </div>
    </div>
  );
};

export default AgriSupportPage;
