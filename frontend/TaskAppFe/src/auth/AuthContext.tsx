import React, { createContext, useContext, useMemo, useState } from "react";
import * as authApi from "../api/users";

type AuthState = {
  token: string | null;
  fullName?: string | null;
};

type AuthContextValue = {
  isAuthenticated: boolean;
  token: string | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  fullName?: string | null;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [state, setState] = useState<AuthState>(() => ({
    token: localStorage.getItem("access_token"),
    fullName: localStorage.getItem("full_name"),
  }));

  const value = useMemo<AuthContextValue>(() => {
    return {
      isAuthenticated: !!state.token,
      token: state.token,
      login: async (email, password) => {
        const res = await authApi.loginUser({ email, password });
        localStorage.setItem("access_token", res.accessToken);
        localStorage.setItem("full_name", res.fullName ?? "");
        setState({ token: res.accessToken, fullName: res.fullName });
      },
      logout: () => {
        localStorage.removeItem("access_token");
        localStorage.removeItem("full_name");
        setState({ token: null, fullName: null });
      },
      fullName: state.fullName,
    };
  }, [state.token]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}