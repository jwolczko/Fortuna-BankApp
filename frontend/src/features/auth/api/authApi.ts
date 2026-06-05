import type { LoginRequest, LoginResponse, RegisterRequest } from '../types/auth.types';
import { apiRequest } from '../../../app/apiClient';

type LoginApiResponse = {
  token: string;
  expiresAtUtc: string;
};

type RegisterApiResponse = {
  customerId: string;
};

export async function loginRequest(payload: LoginRequest): Promise<LoginResponse> {
  const response = await apiRequest<LoginApiResponse>('/api/customers/login', {
    method: 'POST',
    body: JSON.stringify({
      email: payload.email,
      password: payload.password,
    }),
  });

  return {
    token: response.token,
    expiresAtUtc: response.expiresAtUtc,
  };
}

export async function registerRequest(payload: RegisterRequest): Promise<{ customerId: string }> {
  const response = await apiRequest<RegisterApiResponse>('/api/customers', {
    method: 'POST',
    body: JSON.stringify({
      firstName: payload.firstName,
      lastName: payload.lastName,
      email: payload.email,
      password: payload.password,
      customerType: payload.customerType,
    }),
  });

  return {
    customerId: response.customerId,
  };
}

export async function logoutRequest(): Promise<void> {
  return Promise.resolve();
}
