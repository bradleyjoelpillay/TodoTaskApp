import { apiRequest } from "./client";
import type { CreateTaskRequest, ListTasksParams, Task, UpdateTaskRequest } from "../types/task";
import type { PagedResult } from "../types/pagedResult";

const ROOT = "/api/tasks";

export function listTasks(params: ListTasksParams): Promise<PagedResult<Task>> {
  const usp = new URLSearchParams();
  usp.set("pageNumber", String(params.pageNumber));
  usp.set("pageSize", String(params.pageSize));

  return apiRequest<PagedResult<Task>>(`${ROOT}?${usp.toString()}`);
}

export function getTask(id: string): Promise<Task> {
  return apiRequest<Task>(`${ROOT}/${encodeURIComponent(id)}`);
}

export function createTask(req: CreateTaskRequest): Promise<Task> {
  return apiRequest<Task>(ROOT, {
    method: "POST",
    body: JSON.stringify(req),
  });
}

export function updateTask(id: string, req: UpdateTaskRequest): Promise<Task> {
  return apiRequest<Task>(`${ROOT}/${encodeURIComponent(id)}`, {
    method: "PUT",
    body: JSON.stringify(req),
  });
}

export function moveTaskToNextStatus(id: string): Promise<Task> {
  return apiRequest<Task>(`${ROOT}/MoveToNextStatus/${encodeURIComponent(id)}`, {
    method: "PUT",
    body: JSON.stringify({ }),
  });
}

export function deleteTask(id: string): Promise<void> {
  return apiRequest<void>(`${ROOT}/${encodeURIComponent(id)}`, {
    method: "DELETE"
  });
}