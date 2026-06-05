const DEFAULT_API_BASE_URL = 'https://localhost:57751';

export function getApiBaseUrl() {
  const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL;

  return configuredBaseUrl?.trim() || DEFAULT_API_BASE_URL;
}

export async function apiRequest<TResponse>(path: string, init?: RequestInit): Promise<TResponse> {
  const response = await fetch(`${getApiBaseUrl()}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    const fallbackMessage = `Request failed with status ${response.status}.`;

    try {
      const data = (await response.json()) as { detail?: string; message?: string; title?: string };
      throw new Error(data.detail || data.message || data.title || fallbackMessage);
    } catch (error) {
      if (error instanceof Error && error.message !== 'Unexpected end of JSON input') {
        throw error;
      }

      throw new Error(fallbackMessage);
    }
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return (await response.json()) as TResponse;
}
