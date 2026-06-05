import { apiRequest } from '../../../app/apiClient';
import type { CreateTransferRequest } from '../types/transfer.types';

export async function createTransferRequest(token: string, payload: CreateTransferRequest) {
  return apiRequest<string>('/api/transfers', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      transferType: payload.transferType,
      sourceAccountId: payload.sourceAccountId,
      targetAccountId: payload.targetAccountId ?? null,
      targetAccountNumber: payload.targetAccountNumber ?? null,
      recipientName: payload.recipientName ?? null,
      amount: payload.amount,
      currency: payload.currency,
      title: payload.title,
    }),
  });
}
