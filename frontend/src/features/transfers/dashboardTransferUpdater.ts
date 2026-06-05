import type { QueryClient } from '@tanstack/react-query';
import type { DashboardData, DashboardEvent } from '../dashboard/types/dashboard.types';
import type { CreateTransferRequest } from './types/transfer.types';

function createDashboardEvent(
  title: string,
  amount: number,
  currency: string,
  isPositive: boolean,
): DashboardEvent {
  return {
    id: crypto.randomUUID(),
    eventDateUtc: new Date().toISOString(),
    eventType: isPositive ? 'deposit' : 'withdrawal',
    title,
    amount,
    currency,
    isPositive,
  };
}

export function updateDashboardAfterTransfer(
  queryClient: QueryClient,
  customerId: string,
  transfer: CreateTransferRequest,
) {
  queryClient.setQueryData<DashboardData>(['dashboard', customerId], (currentDashboard) => {
    if (!currentDashboard) {
      return currentDashboard;
    }

    const updatedProducts = currentDashboard.products.map((product) => {
      if (product.productId === transfer.sourceAccountId) {
        return {
          ...product,
          balance: product.balance - transfer.amount,
        };
      }

      if (transfer.transferType === 'Own' && product.productId === transfer.targetAccountId) {
        return {
          ...product,
          balance: product.balance + transfer.amount,
        };
      }

      return product;
    });

    const newEvents: DashboardEvent[] =
      transfer.transferType === 'Own'
        ? [
            createDashboardEvent(transfer.title, transfer.amount, transfer.currency, false),
            createDashboardEvent(transfer.title, transfer.amount, transfer.currency, true),
          ]
        : [createDashboardEvent(transfer.title, transfer.amount, transfer.currency, false)];

    return {
      ...currentDashboard,
      totalBalance:
        transfer.transferType === 'Own'
          ? currentDashboard.totalBalance
          : currentDashboard.totalBalance - transfer.amount,
      products: updatedProducts,
      events: [...newEvents, ...currentDashboard.events].slice(0, 20),
    };
  });
}
