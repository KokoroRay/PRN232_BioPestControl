import axios from 'axios';
import { getToken } from '../lib/http';

const BASE = import.meta.env.VITE_WEB_MVC_API ?? 'http://localhost:5000';

const client = axios.create({ baseURL: BASE });

client.interceptors.request.use((config) => {
  const token = getToken();
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface ProductOption {
  id: number;
  name: string;
  imageUrl?: string;
}

export interface CropOption {
  id: number;
  name: string;
}

export interface PestOption {
  id: number;
  name: string;
}

export interface DosageResult {
  productName: string;
  dosageRate: string;
  totalProductNeeded: string;
  numberOfTanks: number;
  amountPerTank: string;
}

export interface MixabilityResult {
  isSafe: boolean;
  warnings?: string[];
}

export interface WeatherInfo {
  province: string;
  district: string;
  temperature: number;
  humidity: number;
  windSpeed: number;
  description: string;
  advice: string;
}

export interface ProvinceDto {
  id: number;
  name: string;
}

export interface DistrictDto {
  id: number;
  name: string;
}

export const agriculturalService = {
  /** Load dropdown data: products, crops, pests */
  getCalculationsData: async () => {
    const { data } = await client.get('/AgriCalculations');
    return data;
  },

  /** POST dosage calculation */
  calculateDosage: async (body: {
    productId: number;
    cropId: number;
    pestId: number;
    areaSize: number;
    tankCapacity: number;
  }) => {
    const { data } = await client.post('/AgriCalculations/Dosage', body);
    return data;
  },

  /** POST mixability check */
  checkMixability: async (productIds: number[]) => {
    const { data } = await client.post('/AgriCalculations/Mixability', { mixProductIds: productIds });
    return data;
  },

  /** GET weather by location */
  getWeatherByLocation: async (provinceId: number, districtId: number) => {
    const { data } = await client.post('/AgriSupport/ByLocation', null, {
      params: { provinceId, districtId },
    });
    return data as WeatherInfo;
  },

  /** GET districts for a province */
  getDistricts: async (provinceId: number) => {
    const { data } = await client.get('/AgriSupport/Districts', {
      params: { provinceId },
    });
    return data as DistrictDto[];
  },

  /** POST weather by authenticated user's address */
  getWeatherByMyAddress: async () => {
    const { data } = await client.post('/AgriSupport/MyAddress');
    return data as WeatherInfo;
  },

  /** GET provinces list */
  getProvinces: async () => {
    const { data } = await client.get('/AgriSupport');
    return data;
  },
};
