import React from "react";
import type { TaskStatus } from "../types/task";

export function StatusBadge({ status }: { status: TaskStatus }) {
  const isDone = status === "Done";

  return (
    <span
      style={{
        ...styles.badge,
        background: isDone ? "rgba(3, 97, 18, 0.64)" : "rgba(255,255,255,0.06)",
      }}
    >
      {status === "Todo" ? "To do" : (status === "InProgress" ? "In progress" : "Done")}
    </span>
  );
}

const styles: Record<string, React.CSSProperties> = {
  badge: {
    padding: "4px 10px",
    borderRadius: 999,
    border: "1px solid rgba(255,255,255,0.12)",
    fontSize: 12,
    whiteSpace: "nowrap",
  },
};