import { useQuery } from '@tanstack/react-query';
import { useAppSelector } from '../../../app/store/hooks';
import { getDashboardData } from '../api/dashboardApi';

export function useDashboard() {
  const token = useAppSelector((state) => state.auth.token);
  const customerId = useAppSelector((state) => state.auth.customerId);

  return useQuery({
    queryKey: ['dashboard', customerId],
    queryFn: () => getDashboardData(token!, customerId!),
    enabled: Boolean(token && customerId),
  });
}
