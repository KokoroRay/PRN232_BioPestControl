import axios from 'axios';
import { API } from '../config/api';

const API_URL = `${API.catalog}/api`;

export interface CropProductDetail {
    productId: number;
    productName: string;
    productImageUrl?: string;
    usageInstruction: string;
    categoryId: number;
    categoryName: string;
}

export interface CropResponse {
    id: number;
    name: string;
    slug: string;
    description?: string;
    imageUrl?: string;
    isActive: boolean;
}

export interface CropProfileResponse extends CropResponse {
    products: CropProductDetail[];
}

export const cropService = {
    getAllCrops: async (): Promise<CropResponse[]> => {
        const response = await axios.get(`${API_URL}/crops`);
        return response.data;
    },

    getCropById: async (id: number): Promise<CropProfileResponse> => {
        const response = await axios.get(`${API_URL}/crops/${id}`);
        return response.data;
    },

    getCropBySlug: async (slug: string): Promise<CropProfileResponse> => {
        const response = await axios.get(`${API_URL}/crops/slug/${slug}`);
        return response.data;
    }
};
