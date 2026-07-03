import { API } from '../config/api';
import { createApiClient } from '../lib/http';

const client = createApiClient(`${API.agriExpert}/api/ai`);

export interface AiResponse {
  success: boolean;
  response: string;
  errorMessage?: string;
}

export const aiService = {
  chat: async (message: string, images?: string[]): Promise<AiResponse> => {
    const { data } = await client.post('/chat', { message, images: images || [] });
    return data;
  }
};
