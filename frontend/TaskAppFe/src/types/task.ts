export type TaskStatus = "Todo" | "InProgress" | "Done";

export type Task = {
  id: string;     
  title: string;
  description: string;
  status: TaskStatus;
  createdAtUtc: string;
  modifiedAtUtc: string;
  userId: string;
};

export type CreateTaskRequest = {
  title: string;
  description?: string;
};

export type UpdateTaskRequest = {
  id: string;
  title?: string;
  description?: string;
};

export type ListTasksParams = {
  pageNumber: number;
  pageSize: number;
};