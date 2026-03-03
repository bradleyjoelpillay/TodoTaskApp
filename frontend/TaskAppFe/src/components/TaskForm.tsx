import React, { useRef, useState } from "react";

export function TaskForm({ onAdd }: { onAdd: (title: string, description: string) => Promise<void> | void }) {
  const [draft, setDraft] = useState("");
  const [description, setDescription] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const [titleError, setTitleError] = useState<string | null>(null);
  const [descriptionError, setDescriptionError] = useState<string | null>(null);

  const inputRef = useRef<HTMLInputElement | null>(null);
  const descriptionRef = useRef<HTMLInputElement | null>(null);

  async function submit() {
    const title = draft.trim();
    let hasError = false;

    if (!title) {
      setTitleError("Title is required.");
      hasError = true;
    } else {
      setTitleError(null);
    }

    if (!description.trim()) {
      setDescriptionError("Description is required.");
      hasError = true;
    } else {
      setDescriptionError(null);
    }

    if (hasError) return;

    setSubmitting(true);
    try {
      await onAdd(title, description);
      setDraft("");
      setDescription("");
      inputRef.current?.focus();
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <>
      <div style={styles.row}>
        <input
          ref={inputRef}
          style={{
            ...styles.input,
            ...(titleError ? styles.inputError : {})
          }}
          value={draft}
          required
          onChange={(e) => {
            setDraft(e.target.value)
            if (titleError) setTitleError(null);
          }}
          placeholder="Title"
          onKeyDown={(e) => {
            if (e.key === "Enter") void submit();
            if (e.key === "Escape") setDraft("");
          }}
        />

      </div>
      <div style={styles.row}>
        {titleError && <div style={styles.errorText}>{titleError}</div>}
      </div>
      <div style={{ ...styles.row, ...styles.mt3 }}>
        <input
          ref={descriptionRef}
          style={{
            ...styles.input,
            ...(descriptionError ? styles.inputError : {})
          }}
          value={description}
          required
          onChange={(e) => {
            setDescription(e.target.value);
            if (descriptionError) setDescriptionError(null);
          }}
          placeholder="Description"
          onKeyDown={(e) => {
            if (e.key === "Enter") void submit();
            if (e.key === "Escape") setDescription("");
          }}
        />

      </div>
      <div style={styles.row}>
        {descriptionError && (
          <div style={styles.errorText}>{descriptionError}</div>
        )}
      </div>
      <div style={{ ...styles.row, ...styles.mt3 }}>
        <button style={{ ...styles.btn, ...styles.primary }} onClick={() => void submit()} disabled={submitting} aria-label="Add task">
          Add
        </button>
      </div>
    </>
  );
}

const styles: Record<string, React.CSSProperties> = {
  mt3: { marginTop: 12 },
  row: { display: "flex", gap: 10, alignItems: "center" },
  input: {
    width: "100%",
    padding: "10px 12px",
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(0,0,0,0.15)",
    color: "inherit",
    outline: "none",
  },
  inputError: {
    border: "1px solid #ff4d4f",
  },
  errorText: {
    color: "#ff4d4f",
    fontSize: 12,
    marginTop: 6,
  },
  btn: {
    width: "100%",
    height: 40,
    borderRadius: 14,
    border: "1px solid rgba(255,255,255,0.12)",
    background: "rgba(255,255,255,0.06)",
    color: "inherit",
    cursor: "pointer",
    display: "inline-flex",
    alignItems: "center",
    justifyContent: "center",
  },
  primary: { background: "rgba(255,255,255,0.12)" },
};