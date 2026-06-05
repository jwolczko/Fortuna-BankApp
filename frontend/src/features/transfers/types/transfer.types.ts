export type TransferMode = 'Own' | 'External';

export type CreateTransferRequest = {
  transferType: TransferMode;
  sourceAccountId: string;
  targetAccountId?: string;
  targetAccountNumber?: string;
  recipientName?: string;
  amount: number;
  currency: string;
  title: string;
};
