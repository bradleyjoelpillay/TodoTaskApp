import React from "react";
import { AnimatePresence, motion } from "framer-motion";
import type { Task } from "../types/task";
import { TaskCard } from "./TaskCard";

export function TaskList({
  tasks,
  onToggle,
  onDelete,
  onEditTask,
}: {
  tasks: Task[];
  onToggle: (task: Task) => void;
  onDelete: (task: Task) => void;
  onEditTask: (task: Task, title: string, description: string) => void;
  onOpen?: (task: Task) => void;
}) {
  if (tasks.length === 0) {
    return (
      <div style={styles.empty}>
        No tasks found.
      </div>
    );
  }

  return (
    <ul style={styles.list}>
      <AnimatePresence initial={false}>
        {tasks.map((t) => (
          <motion.div
            key={t.id}
            layout
            initial={{ opacity: 0, y: 8 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -8 }}
            transition={{ duration: 0.2 }}
          >
            <TaskCard task={t} onToggle={onToggle} onDelete={onDelete} onEditTask={onEditTask} />
          </motion.div>
        ))}
      </AnimatePresence>
    </ul>
  );
}

const styles: Record<string, React.CSSProperties> = {
  list: { margin: 0, padding: 0, display: "flex", flexDirection: "column", gap: 10 },
  empty: {
    border: "1px dashed rgba(255,255,255,0.14)",
    borderRadius: 16,
    padding: 22,
    textAlign: "center",
    opacity: 0.75,
  },
};