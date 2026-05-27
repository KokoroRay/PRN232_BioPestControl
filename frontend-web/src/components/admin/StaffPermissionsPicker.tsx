import React from 'react';
import type { PermissionGroup } from '../../types/identity';

interface StaffPermissionsPickerProps {
  groups: PermissionGroup[];
  selectedIds: number[];
  disabled?: boolean;
  onChange: (ids: number[]) => void;
}

export const StaffPermissionsPicker: React.FC<StaffPermissionsPickerProps> = ({
  groups,
  selectedIds,
  disabled,
  onChange,
}) => {
  const toggle = (id: number) => {
    if (disabled) return;
    onChange(
      selectedIds.includes(id)
        ? selectedIds.filter((x) => x !== id)
        : [...selectedIds, id],
    );
  };

  if (!groups.length) {
    return <p className="text-muted">No permissions loaded.</p>;
  }

  return (
    <div className="permission-groups">
      {groups.map((g) => (
        <section key={g.groupCode} className="permission-group">
          <h4>{g.groupName}</h4>
          <div className="permission-list">
            {g.permissions.map((p) => (
              <label key={p.id} className="checkbox-row permission-item">
                <input
                  type="checkbox"
                  checked={selectedIds.includes(p.id)}
                  disabled={disabled}
                  onChange={() => toggle(p.id)}
                />
                <span>
                  <strong>{p.displayName}</strong>
                  {p.description ? (
                    <span className="text-muted permission-desc"> — {p.description}</span>
                  ) : null}
                </span>
              </label>
            ))}
          </div>
        </section>
      ))}
    </div>
  );
};
