import type { DashboardData } from '../types/dashboard.types';
import { apiRequest } from '../../../app/apiClient';

export async function getDashboardData(token: string, customerId: string): Promise<DashboardData> {
  return apiRequest<DashboardData>(`/api/dashboard/${customerId}`, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}
