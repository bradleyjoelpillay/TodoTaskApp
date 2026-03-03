import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../auth/AuthContext";

type FieldErrors = {
  email?: string;
  password?: string;
};

export function LoginPage() {
  const nav = useNavigate();
  const { login } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [errors, setErrors] = useState<FieldErrors>({});
  const [serverError, setServerError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  function validate(): boolean {
    const newErrors: FieldErrors = {};

    if (!email.trim()) {
      newErrors.email = "Email is required.";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = "Please enter a valid email address.";
    }

    if (!password.trim()) {
      newErrors.password = "Password is required.";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setServerError(null);

    if (!validate()) return;

    setLoading(true);

    try {
      await login(email, password);
      nav("/tasks", { replace: true });
    } catch (err: any) {
      setServerError(
        err?.response?.data?.title ?? err?.message ?? "Login failed"
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: "40px auto" }}>
      <h2>Login</h2>

      {serverError && (
        <div style={{ color: "crimson", marginBottom: 12 }}>
          {serverError}
        </div>
      )}

      <form onSubmit={onSubmit} noValidate>
        <div style={{ display: "grid", gap: 6 }}>
          <input
            style={styles.input}
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          {errors.email && (
            <span style={styles.error}>{errors.email}</span>
          )}

          <input
            style={styles.input}
            placeholder="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {errors.password && (
            <span style={styles.error}>{errors.password}</span>
          )}

          <button style={styles.btn} disabled={loading} type="submit">
            {loading ? "Signing in..." : "Sign in"}
          </button>
        </div>
      </form>

      <p style={{ marginTop: 12 }}>
        No account? <Link to="/register">Register</Link>
      </p>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  input: {
    padding: "10px 12px",
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(0,0,0,0.15)",
    color: "inherit",
    outline: "none",
  },
  btn: {
    display: "inline-flex",
    gap: 8,
    alignItems: "center",
    justifyContent: "center",
    padding: "10px 12px",
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.06)",
    background: "rgba(255,255,255,0.06)",
    color: "inherit",
    cursor: "pointer",
  },
  error: {
    color: "crimson",
    fontSize: 12,
    marginBottom: 4,
  },
};