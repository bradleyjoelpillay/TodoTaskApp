import { useCallback, useEffect, useState } from "react";
import type { ApiError } from "../api/client";
import type { Task, TaskStatus } from "../types/task";
import * as tasksApi from "../api/tasks";

export type FilterMode = "all" | "todo" | "inprogress" | "done";
export type SortMode = "newest" | "oldest" | "az";

export function useTasks() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<ApiError | null>(null);

  const [pageNumber, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await tasksApi.listTasks({ pageNumber: pageNumber, pageSize });
      setTasks(data.items);
      setTotalCount(data.totalCount);
      setTotalPages(data.totalPages);
    } catch (e) {
      setError(e as ApiError);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const add = useCallback(async (title: string, description: string) => {
    setError(null);
    const trimmed = title.trim();
    if (!trimmed) return;

    try {
      const created = await tasksApi.createTask({ title: trimmed, description });
      setPage(1);
      setTasks((prev) => [created, ...prev]);
      setTotalCount(prevCount => {
        const nextCount = Math.max(0, prevCount + 1);
        const nextTotalPages = Math.max(1, Math.ceil(nextCount / pageSize));

        setPage(prevPage => Math.min(prevPage, nextTotalPages));

        return nextCount;
      });

    } catch (e) {
      setError(e as ApiError);
      throw e;
    }
  }, []);

  const toggleStatus = useCallback(async (task: Task) => {
    setError(null);
    const nextStatus: TaskStatus = task.status === "Done" ? "InProgress" : "Done";

    setTasks((prev) => prev.map((t) => (t.id === task.id ? { ...t, status: nextStatus } : t)));

    try {
      const updated = await tasksApi.moveTaskToNextStatus(task.id);
      setTasks((prev) => prev.map((t) => (t.id === task.id ? updated : t)));
    } catch (e) {
      setTasks((prev) => prev.map((t) => (t.id === task.id ? task : t)));
      setError(e as ApiError);
      throw e;
    }
  }, []);

  const editTask = useCallback(async (task: Task, title: string, description: string) => {
    setError(null);
    const trimmed = title.trim();
    if (!trimmed) return;

    const before = task;
    setTasks((prev) => prev.map((t) => (t.id === task.id ? { ...t, title: trimmed, description } : t)));

    try {
      const updated = await tasksApi.updateTask(task.id,{ id: task.id, title: trimmed, description });
      setTasks((prev) => prev.map((t) => (t.id === task.id ? updated : t)));
    } catch (e) {
      setTasks((prev) => prev.map((t) => (t.id === task.id ? before : t)));
      setError(e as ApiError);
      throw e;
    }
  }, []);

  const remove = useCallback(async (task: Task) => {
    setError(null);
    const before = tasks;

    setTasks((prev) => prev.filter((t) => t.id !== task.id));

    try {
      await tasksApi.deleteTask(task.id);
      setTotalCount(prevCount => {
        const nextCount = Math.max(0, prevCount - 1);
        const nextTotalPages = Math.max(1, Math.ceil(nextCount / pageSize));

        setPage(prevPage => Math.min(prevPage, nextTotalPages));

        return nextCount;
      });

    } catch (e) {
      setTasks(before);
      setError(e as ApiError);
      throw e;
    }
  }, [tasks, pageSize]);

  return {
    // data
    tasks,
    loading,
    error,
    
    // actions
    refresh,
    add,
    toggleStatus,
    editTask,
    remove,
    setError,

    pageNumber,
    totalCount,
    totalPages,
    setPage,

  };
}