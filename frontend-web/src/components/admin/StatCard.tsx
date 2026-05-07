import React from 'react';
import type { LucideIcon } from 'lucide-react';

interface StatCardProps {
  title: string;
  value: string | number;
  subtitle?: string;
  icon: LucideIcon;
  iconBgColor: string;
  iconColor: string;
  growth?: number;
}

export const StatCard: React.FC<StatCardProps> = ({
  title,
  value,
  subtitle,
  icon: Icon,
  iconBgColor,
  iconColor,
  growth,
}) => {
  return (
    <div className="stat-card">
      <div className="stat-card-header">
        <div className="stat-icon" style={{ backgroundColor: iconBgColor }}>
          <Icon size={24} color={iconColor} />
        </div>
        {growth !== undefined && growth !== 0 && (
          <span className={`growth-badge ${growth >= 0 ? 'growth-up' : 'growth-down'}`}>
            {growth >= 0 ? "↑" : "↓"} {Math.abs(growth).toFixed(1)}%
          </span>
        )}
      </div>
      <div className="stat-label">{title}</div>
      <div className="stat-value">{value}</div>
      {subtitle && <div className="stat-subtitle">{subtitle}</div>}
    </div>
  );
};
