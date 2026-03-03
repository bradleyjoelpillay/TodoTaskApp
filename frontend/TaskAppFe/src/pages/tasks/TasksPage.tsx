import React from "react";
import { useTasks } from "../../hooks/useTasks";
import { ErrorBanner } from "../../components/ErrorBanner";
import { TaskForm } from "../../components/TaskForm";
import { TaskList } from "../../components/TaskList";
import { useAuth } from "../../auth/AuthContext";

export function TasksPage({ onOpenTask }: { onOpenTask?: (id: string) => void }) {
  const {
    tasks,
    loading,
    error,
    add,
    toggleStatus,
    editTask,
    remove,
    setError,
    totalCount,
    pageNumber,
    totalPages,
    setPage,
  } = useTasks();

  const { logout, fullName } = useAuth();

  return (
    <div style={styles.page}>
      <div style={styles.container}>
        <div style={styles.header}>
          <div>
            <h1 style={{ margin: 0 }}>My Todo Tasks</h1>
          </div>

          <div style={styles.badges}>
            <span style={{paddingTop:8}}>Welcome, {fullName}</span>
            <button
              style={styles.btn}
              onClick={logout}
            >
              Logout
            </button>
          </div>
        </div>

        <ErrorBanner error={error} onDismiss={() => setError(null)} />

        <section style={styles.card}>
          <h2 style={styles.cardTitle}>Add a task</h2>
          <TaskForm onAdd={add} />

          <div style={{ height: 12 }} />
        </section>

        <section style={styles.card}>
          <h2 style={styles.cardTitle}>List</h2>
          {loading ? (
            <div style={styles.muted}>Loading…</div>
          ) : (
            <TaskList
              tasks={tasks}
              onToggle={(t) => void toggleStatus(t)}
              onDelete={(t) => void remove(t)}
              onEditTask={(t, title, description) => void editTask(t, title, description)}
              onOpen={(t) => onOpenTask?.(t.id)}
            />
          )}
        </section>

        <div style={{ display: "flex", gap: 10, alignItems: "center", justifyContent: "space-between", flexWrap: "wrap" }}>
          <div style={{ opacity: 0.8 }}>
            Page {pageNumber} of {totalPages} • Total {totalCount}
          </div>

          <div style={{ display: "flex", gap: 10 }}>
            <button
              style={styles.btn}
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={pageNumber <= 1 || loading}
            >
              ← Prev
            </button>

            <button
              style={styles.btn}
              onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              disabled={pageNumber >= totalPages || loading}
            >
              Next →
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: { minHeight: "100vh", width: "100%" },
  container: { maxWidth: "100%", margin: "0 auto", padding: 24, display: "flex", flexDirection: "column", gap: 16 },
  header: { display: "flex", alignItems: "flex-start", justifyContent: "space-between", gap: 16, flexWrap: "wrap" },
  badges: { display: "flex", gap: 8, flexWrap: "wrap" },
  badge: {
    padding: "6px 10px",
    borderRadius: 999,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(255,255,255,0.06)",
    fontSize: 12,
  },
  card: {
    border: "1px solid rgba(255,255,255,0.10)",
    background: "rgba(0,0,0,0.10)",
    borderRadius: 5,
    padding: 16,
  },
  cardTitle: { margin: "0 0 12px", fontSize: 16 },
  toolbar: { display: "flex", gap: 12, alignItems: "center", flexWrap: "wrap" },
  controls: { display: "flex", gap: 10, alignItems: "center", flexWrap: "wrap" },
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
  selectWrap: {
    display: "inline-flex",
    gap: 8,
    alignItems: "center",
    padding: "0 10px",
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(255,255,255,0.06)",
    height: 40,
  },
  select: { border: "none", background: "transparent", color: "inherit", outline: "none", height: 36 },
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
  danger: { background: "rgba(255,80,80,0.18)", borderColor: "rgba(255,80,80,0.35)" },
  muted: { opacity: 0.75 },
};