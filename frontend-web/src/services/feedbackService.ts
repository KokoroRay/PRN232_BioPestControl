import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || '/api';

const client = axios.create({
  baseURL: API_URL,
});

export const getFeedbacksByProductId = async (productId: number) => {
  const { data } = await client.get(`/feedbacks/product/${productId}`);
  return data;
};

export const createFeedback = async (feedback: any) => {
  const { data } = await client.post('/feedbacks', feedback);
  return data;
};

export const submitContact = async (contact: any) => {
  const { data } = await client.post('/contacts', contact);
  return data;
};
