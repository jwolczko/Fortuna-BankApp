import type { LoginResponse } from './types/auth.types';

const AUTH_SESSION_KEY = 'fortuna.auth';

export type StoredAuthSession = {
  token: string;
  userName: string;
  customerId: string;
  expiresAtUtc: string;
};

function isBrowser() {
  return typeof window !== 'undefined';
}

export function decodeJwtPayload(token: string) {
  const [, payload] = token.split('.');

  if (!payload) {
    throw new Error('Invalid authentication token.');
  }

  const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
  const paddedPayload = normalizedPayload.padEnd(normalizedPayload.length + ((4 - normalizedPayload.length % 4) % 4), '=');
  const decodedPayload = atob(paddedPayload);

  return JSON.parse(decodedPayload) as Record<string, string>;
}

export function createSessionFromLogin(email: string, response: LoginResponse): StoredAuthSession {
  const payload = decodeJwtPayload(response.token);
  const customerId = payload.customer_id || payload.sub;

  if (!customerId) {
    throw new Error('Customer identifier was not found in the authentication token.');
  }

  return {
    token: response.token,
    userName: payload.email || email,
    customerId,
    expiresAtUtc: response.expiresAtUtc,
  };
}

export function saveAuthSession(session: StoredAuthSession) {
  if (!isBrowser()) {
    return;
  }

  sessionStorage.setItem(AUTH_SESSION_KEY, JSON.stringify(session));
}

export function loadAuthSession(): StoredAuthSession | null {
  if (!isBrowser()) {
    return null;
  }

  const storedSession = sessionStorage.getItem(AUTH_SESSION_KEY);

  if (!storedSession) {
    return null;
  }

  try {
    const session = JSON.parse(storedSession) as StoredAuthSession;

    if (!session.token || !session.customerId) {
      clearAuthSession();
      return null;
    }

    if (session.expiresAtUtc && new Date(session.expiresAtUtc).getTime() <= Date.now()) {
      clearAuthSession();
      return null;
    }

    return session;
  } catch {
    clearAuthSession();
    return null;
  }
}

export function clearAuthSession() {
  if (!isBrowser()) {
    return;
  }

  sessionStorage.removeItem(AUTH_SESSION_KEY);
}
