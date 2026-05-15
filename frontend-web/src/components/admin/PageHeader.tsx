import React from 'react';
import { Plus } from 'lucide-react';

interface PageHeaderProps {
  title: string;
  subtitle?: string;
  actionLabel?: string;
  onAction?: () => void;
}

export const PageHeader: React.FC<PageHeaderProps> = ({
  title,
  subtitle,
  actionLabel,
  onAction,
}) => (
  <div className="page-header">
    <div>
      <h1 className="page-title">{title}</h1>
      {subtitle && <p className="page-subtitle">{subtitle}</p>}
    </div>
    {actionLabel && onAction && (
      <button type="button" className="btn-primary" onClick={onAction}>
        <Plus size={20} />
        {actionLabel}
      </button>
    )}
  </div>
);
