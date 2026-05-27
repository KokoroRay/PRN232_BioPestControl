import React from 'react';
import { X } from 'lucide-react';

interface DrawerProps {
  open: boolean;
  title: string;
  onClose: () => void;
  children: React.ReactNode;
  wide?: boolean;
}

export const Drawer: React.FC<DrawerProps> = ({ open, title, onClose, children, wide }) => {
  if (!open) return null;
  const overlay = 'drawer-overlay';
  return (
    <>
      <div className={overlay} onClick={onClose} aria-hidden />
      <aside className={`drawer-panel ${wide ? 'drawer-wide' : ''}`}>
        <div className="drawer-header">
          <h2>{title}</h2>
          <button type="button" className="icon-btn" onClick={onClose} aria-label="Close">
            <X size={22} />
          </button>
        </div>
        <div className="drawer-body">{children}</div>
      </aside>
    </>
  );
};
