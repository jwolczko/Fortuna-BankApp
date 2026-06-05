import type { QueryClient } from '@tanstack/react-query';
import type { AppDispatch } from '../../app/store/store';
import { getDashboardData } from '../dashboard/api/dashboardApi';
import { createSessionFromLogin, saveAuthSession } from './authSession';
import { loginRequest } from './api/authApi';
import { setCredentials } from './store/authSlice';

export async function loginAndInitializeSession(
  email: string,
  password: string,
  dispatch: AppDispatch,
  queryClient: QueryClient,
) {
  const response = await loginRequest({
    email,
    password,
  });

  const session = createSessionFromLogin(email, response);

  saveAuthSession(session);
  dispatch(setCredentials(session));

  await queryClient.prefetchQuery({
    queryKey: ['dashboard', session.customerId],
    queryFn: () => getDashboardData(session.token, session.customerId),
  });

  return session;
}
