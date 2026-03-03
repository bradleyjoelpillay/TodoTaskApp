export type ApiError = {
  status: number;
  message: string;
  details?: unknown;
};

const DEFAULT_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";

async function parseError(res: Response): Promise<ApiError> {
  let body: unknown = undefined;

  try {
    const text = await res.text();
    body = text ? JSON.parse(text) : undefined;
  } catch {
    // ignore
  }

  const message =
    (typeof body === "object" && body && "title" in body && typeof (body as any).title === "string"
      ? (body as any).title
      : undefined) ||
    (typeof body === "object" && body && "message" in body && typeof (body as any).message === "string"
      ? (body as any).message
      : undefined) ||
    `Request failed (${res.status})`;

  return { status: res.status, message, details: body };
}

export async function apiRequest<T>(
  path: string,
  init?: RequestInit & { baseUrl?: string }
): Promise<T> {
  const baseUrl = init?.baseUrl ?? DEFAULT_BASE_URL;

  const token = localStorage.getItem("access_token");

  const res = await fetch(`${baseUrl}${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(init?.headers ?? {}),
    },
  });

  if (!res.ok) throw await parseError(res);

  if (res.status === 204) return undefined as T;

  return (await res.json()) as T;
}