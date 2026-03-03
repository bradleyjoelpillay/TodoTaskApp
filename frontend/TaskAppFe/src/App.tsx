import React from "react";
import { BrowserRouter, Route, Routes, useNavigate, Navigate } from "react-router-dom";
import { TasksPage } from "./pages/tasks/TasksPage";
import { AuthProvider } from "./auth/AuthContext";
import { RequireAuth } from "./auth/RequireAuth";
import { RegisterPage } from "./pages/users/RegisterPage";
import { LoginPage } from "./pages/users/LoginPage";

function TasksRoute() {
  const nav = useNavigate();
  return <TasksPage onOpenTask={(id) => nav(`/tasks/${id}`)} />;
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          {/* Public */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Protected */}
          <Route
            path="/tasks"
            element={
              <RequireAuth>
                <TasksRoute />
              </RequireAuth>
            }
          />

          {/* Default */}
          <Route path="/" element={<Navigate to="/tasks" replace />} />
          <Route path="*" element={<Navigate to="/tasks" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}
