import type { AuthResponse, LoginRequest, RegisterRequest } from "../types/users";
import { apiRequest } from "./client";

const ROOT = "/api/auth";

export function registerUser(req: RegisterRequest): Promise<AuthResponse> {
  return apiRequest<AuthResponse>(`${ROOT}/register`, {
    method: "POST",
    body: JSON.stringify(req),
  });
}

export function loginUser(req: LoginRequest): Promise<AuthResponse> {
  return apiRequest<AuthResponse>(`${ROOT}/login`, {
    method: "POST",
    body: JSON.stringify(req),
  });
}
