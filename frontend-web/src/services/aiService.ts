import { API } from '../config/api';
import { createApiClient } from '../lib/http';

const client = createApiClient(`${API.agriExpert}/api/ai`);

export interface AiResponse {
  success: boolean;
  response: string;
  errorMessage?: string;
}

export const aiService = {
  chat: async (message: string): Promise<AiResponse> => {
    const { data } = await client.post('/chat', { message });
    return data;
  },
  
  analyzeDisease: async (base64Image: string): Promise<AiResponse> => {
    const { data } = await client.post('/analyze-disease', { base64Image });
    return data;
  }
};
