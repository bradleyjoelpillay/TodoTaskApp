import React, { useEffect, useRef, useState } from "react";
import type { Task } from "../types/task";
import { StatusBadge } from "./StatusBadge";
import { Check } from "lucide-react";

export function TaskCard({
  task,
  onToggle,
  onDelete,
  onEditTask,
}: {
  task: Task;
  onToggle: (task: Task) => void;
  onDelete: (task: Task) => void;
  onEditTask: (task: Task, title: string, description: string) => void;
}) {
  const [editing, setEditing] = useState(false);
  const [draft, setDraft] = useState(task.title);
  const [description, setDescription] = useState(task.description);
  const inputRef = useRef<HTMLInputElement | null>(null);
  const descriptionInputRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    setDraft(task.title);
  }, [task.title]);

  useEffect(() => {
    if (editing) setTimeout(() => inputRef.current?.focus(), 0);
  }, [editing]);

  function commit() {
    const trimmed = draft.trim();
    if (!trimmed) return;
    onEditTask(task, trimmed, description);
    setEditing(false);
  }

  return (
    <li style={styles.item}>
      <div style={styles.row}>
        <button
          style={{ ...styles.toggle, ...(task.status === "Done" ? styles.toggleDone : (task.status === "InProgress" ? styles.toggleInProgress : styles.toggleTodo)) }}
          onClick={() => onToggle(task)}
          aria-label={task.status === "Todo" ? "Mark as active" : "Mark as done"}
        >
          {task.status === "Done" ? <Check size={16} /> : <span style={{ width: 16, height: 16 }} />}
        </button>

        <div style={{ flex: 1, minWidth: 0 }}>
          {editing ? (
            <>
              <input
                ref={inputRef}
                style={styles.input}
                value={draft}
                onChange={(e) => setDraft(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") commit();
                  if (e.key === "Escape") {
                    setEditing(false);
                    setDraft(task.title);
                  }
                }}
              />
              <input
                ref={descriptionInputRef}
                style={styles.input}
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") commit();
                  if (e.key === "Escape") {
                    setEditing(false);
                    setDescription(task.description);
                  }
                }}
              />
            </>
          ) : (
            <div style={styles.titleRow}>
              <div>
                <span
                  style={{
                    ...styles.title,
                    textDecoration: task.status === "Done" ? "line-through" : "none",
                    opacity: task.status === "Done" ? 0.7 : 1,
                  }}
                >
                  {task.title}
                </span>
                <span style={{ fontSize: 12, opacity: 0.75 }}>
                  {task.description}
                </span>
              </div>
              <StatusBadge status={task.status} />
            </div>
          )}
        </div>

        <div style={styles.actions}>
          {editing ? (
            <>
              <button style={{ ...styles.btn, ...styles.primary }} onClick={commit}>
                Save
              </button>
              <button
                style={styles.btn}
                onClick={() => {
                  setEditing(false);
                  setDraft(task.title);
                }}
              >
                Cancel
              </button>
            </>
          ) : (
            <>
              <button style={styles.iconBtn} onClick={() => setEditing(true)} aria-label="Edit">
                Edit
              </button>
              <button style={{ ...styles.iconBtn, ...styles.danger }} onClick={() => onDelete(task)} aria-label="Delete">
                Delete
              </button>
            </>
          )}
        </div>
      </div>

      {!editing && (
        <div style={styles.meta}>
          Created {new Date(task.createdAtUtc).toLocaleString()}
        </div>
      )}
    </li>
  );
}

const styles: Record<string, React.CSSProperties> = {
  item: {
    border: "1px solid rgba(255,255,255,0.10)",
    background: "rgba(0,0,0,0.10)",
    borderRadius: 16,
    padding: 12,
    listStyle: "none",
  },
  row: { display: "flex", alignItems: "center", gap: 10 },
  toggle: {
    width: 38,
    height: 38,
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(255,255,255,0.05)",
    color: "inherit",
    cursor: "pointer",
    display: "inline-flex",
    alignItems: "center",
    justifyContent: "center",
  },
  toggleDone: { background: "rgb(3, 255, 62)" },
  toggleInProgress: { background: "rgb(10, 10, 192)" },
  titleRow: { display: "flex", alignItems: "center", justifyContent: "space-between", gap: 10 },
  titleBtn: {
    border: "none",
    background: "transparent",
    color: "inherit",
    padding: 0,
    cursor: "pointer",
    textAlign: "left",
    flex: 1,
    minWidth: 0,
  },
  title: {
    display: "block",
    overflow: "hidden",
    textOverflow: "ellipsis",
    whiteSpace: "nowrap",
  },
  actions: { display: "flex", gap: 8, alignItems: "center" },
  iconBtn: {
    width: "inherit",
    height: 38,
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(255,255,255,0.06)",
    color: "inherit",
    cursor: "pointer",
    display: "inline-flex",
    alignItems: "center",
    justifyContent: "center",
  },
  danger: { background: "rgba(255,80,80,0.18)", borderColor: "rgba(255,80,80,0.35)" },
  btn: {
    borderRadius: 12,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(255,255,255,0.06)",
    color: "rgba(255,255,255,0.95)",
    cursor: "pointer",
    padding: "8px 10px",
    fontSize: 12,
  },
  primary: { background: "rgba(255,255,255,0.12)" },
  input: {
    width: "100%",
    padding: "10px 12px",
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(0,0,0,0.15)",
    color: "inherit",
    outline: "none",
  },
  meta: { marginTop: 8, fontSize: 12, opacity: 0.7 },
};