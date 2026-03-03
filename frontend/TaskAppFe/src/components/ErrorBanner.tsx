import React from "react";
import type { ApiError } from "../api/client";

export function ErrorBanner({
  error,
  onDismiss,
}: {
  error: ApiError | null;
  onDismiss?: () => void;
}) {
  if (!error) return null;

  return (
    <div style={styles.wrap} role="alert">
      <div style={styles.text}>
        <strong style={{ display: "block" }}>Something went wrong</strong>
        <span>
          {error.message} <span style={{ opacity: 0.75 }}>(HTTP {error.status})</span>
        </span>
      </div>
      {onDismiss && (
        <button style={styles.btn} onClick={onDismiss} aria-label="Dismiss">
          ✕
        </button>
      )}
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  wrap: {
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    gap: 12,
    padding: 12,
    borderRadius: 12,
    border: "1px solid rgba(255,80,80,0.35)",
    background: "rgba(255,80,80,0.16)",
  },
  text: { lineHeight: 1.25 },
  btn: {
    border: "1px solid rgba(255,255,255,0.2)",
    background: "rgba(255,255,255,0.06)",
    color: "inherit",
    borderRadius: 10,
    padding: "6px 10px",
    cursor: "pointer",
  },
};