import { API } from '../config/api';
import { createApiClient } from '../lib/http';

const client = createApiClient(`${API.payment}`);

export const paymentService = {
  createPaymentLink: async (amount: number, orderId: string, domain: string) => {
    const { data } = await client.post('/api/payment/create-payment-link', { 
      amount,
      orderId,
      domain
    });
    return data;
  }
};
