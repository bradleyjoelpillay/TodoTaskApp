import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import * as authApi from "../../api/users";

type FieldErrors = {
  firstName?: string;
  lastName?: string;
  email?: string;
  password?: string;
};

export function RegisterPage() {
  const nav = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");

  const [errors, setErrors] = useState<FieldErrors>({});
  const [error, setError] = useState<string | null>(null); 
  const [loading, setLoading] = useState(false);

  function validate(): boolean {
    const newErrors: FieldErrors = {};

    if (!firstName.trim()) newErrors.firstName = "First name is required.";
    if (!lastName.trim()) newErrors.lastName = "Last name is required.";

    if (!email.trim()) {
      newErrors.email = "Email is required.";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = "Please enter a valid email address.";
    }

    if (!password) {
      newErrors.password = "Password is required.";
    } else {
      const problems: string[] = [];

      if (password.length < 8) problems.push("at least 8 characters");
      if (!/[A-Z]/.test(password)) problems.push("1 uppercase letter");
      if (!/[a-z]/.test(password)) problems.push("1 lowercase letter");
      if (!/[0-9]/.test(password)) problems.push("1 digit");
      if (!/[^a-zA-Z0-9]/.test(password)) problems.push("1 special character");

      if (problems.length) {
        newErrors.password = `Password must contain ${problems.join(", ")}.`;
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!validate()) return;

    setLoading(true);
    try {
      await authApi.registerUser({ email, password, firstName, lastName });
      nav("/login");
    } catch (err: any) {
      setError(err?.response?.data?.title ?? err?.message ?? "Registration failed");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: "40px auto" }}>
      <h2>Register</h2>

      {error && <div style={{ color: "crimson", marginBottom: 12 }}>{error}</div>}

      <form onSubmit={onSubmit} noValidate>
        <div style={{ display: "grid", gap: 6 }}>
          <input
            style={styles.input}
            placeholder="First name"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
          />
          {errors.firstName && <span style={styles.error}>{errors.firstName}</span>}

          <input
            style={styles.input}
            placeholder="Last name"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
          />
          {errors.lastName && <span style={styles.error}>{errors.lastName}</span>}

          <input
            style={styles.input}
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          {errors.email && <span style={styles.error}>{errors.email}</span>}

          <input
            style={styles.input}
            placeholder="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {errors.password && <span style={styles.error}>{errors.password}</span>}

          <button style={styles.btn} disabled={loading} type="submit">
            {loading ? "Creating..." : "Create account"}
          </button>
        </div>
      </form>

      <p style={{ marginTop: 12 }}>
        Already have an account? <Link to="/login">Login</Link>
      </p>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  input: {
    flex: 1,
    minWidth: 220,
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
    border: "1px solid rgba(255,255,255,0.12)",
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